using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using System.Collections.Generic;
using Microsoft.VisualBasic; // Для InputBox

namespace AffineTransformations3D
{
    public partial class MainWindow : Window
    {
        private PerspectiveCamera camera;
        private ModelVisual3D prismVisual;
        private DispatcherTimer timer = new DispatcherTimer();
        private double rotationAngle = 1.0;
        private bool rotating = false;
        private double centerX = 0;
        private double centerY = 0;
        private double centerZ = 0;
        private Matrix3D totalTransform = Matrix3D.Identity;

        public MainWindow()
        {
            InitializeComponent();
            InitializeScene();

            timer.Interval = TimeSpan.FromMilliseconds(16);
            timer.Tick += Timer_Tick;
        }

        private void InitializeScene()
        {
            // Создание камеры
            camera = new PerspectiveCamera();
            camera.Position = new Point3D(0, 0, -5);
            camera.LookDirection = new Vector3D(0, 0, 1);
            camera.UpDirection = new Vector3D(0, 1, 0);
            camera.FieldOfView = 45;

            //Получение количества сторон от пользователя
            int n = GetNumberOfSides();
            if (n < 3)
            {
                MessageBox.Show("Количество сторон должно быть не меньше 3.");
                return;
            }

            // Создание модели призмы
            Model3DGroup prismModelGroup = CreatePrism(n);
            prismModelGroup.Transform = new MatrixTransform3D(totalTransform); // Применение трансформации
            prismVisual = new ModelVisual3D();
            prismVisual.Content = prismModelGroup;

            //Создание освещения
            DirectionalLight directionalLight = new DirectionalLight();
            directionalLight.Color = Colors.White;
            directionalLight.Direction = new Vector3D(-0.61, -0.5, -0.61);

            ModelVisual3D lightVisual = new ModelVisual3D();
            lightVisual.Content = directionalLight;

            //Добавление модели и камеры в Viewport3D
            ModelVisual3D ambientLightVisual = new ModelVisual3D();
            ambientLightVisual.Content = new AmbientLight(Colors.Gray);

            MyViewport.Camera = camera;
            MyViewport.Children.Add(prismVisual);
            MyViewport.Children.Add(lightVisual);
            MyViewport.Children.Add(ambientLightVisual);
        }

        //Получение количества сторон призмы от пользователя
        private int GetNumberOfSides()
        {
            string input = Interaction.InputBox("Введите количество сторон основания призмы (n):", "Количество сторон", "6");
            if (int.TryParse(input, out int n))
            {
                return n;
            }
            else
            {
                MessageBox.Show("Некорректный ввод. Будет использовано значение по умолчанию (6).");
                return 6;
            }
        }

        private Model3DGroup CreatePrism(int n)
        {
            Model3DGroup prismModelGroup = new Model3DGroup();
            List<Color> faceColors = new List<Color>
            {
                Colors.Red, Colors.Green, Colors.Blue, Colors.Yellow, Colors.Orange, Colors.Purple,
                Colors.Brown, Colors.Pink, Colors.Teal, Colors.Lime
            };

            double height = 1.0;
            double radius = 0.5;

            //Создаем точки для основания
            Point3D[] basePoints = new Point3D[n];
            for (int i = 0; i < n; i++)
            {
                double angle = 2 * Math.PI * i / n;
                basePoints[i] = new Point3D(radius * Math.Cos(angle), -height / 2, radius * Math.Sin(angle));
                centerX += basePoints[i].X;
                centerY += basePoints[i].Y;
                centerZ += basePoints[i].Z;
            }

            //Создаем вершину призмы
            Point3D topPoint = new Point3D(0, height / 2, 0);
            centerX += topPoint.X;
            centerY += topPoint.Y;
            centerZ += topPoint.Z;

            centerX /= (n + 1);  //n точек основания + 1 вершина
            centerY /= (n + 1);
            centerZ /= (n + 1);

            //Создаем боковые грани
            for (int i = 0; i < n; i++)
            {
                int next = (i + 1) % n;

                MeshGeometry3D mesh = new MeshGeometry3D();
                mesh.Positions.Add(basePoints[i]);
                mesh.Positions.Add(basePoints[next]);
                mesh.Positions.Add(topPoint);  //Используем общую вершину

                //Правильный порядок обхода вершин для лицевой стороны
                mesh.TriangleIndices.Add(0);
                mesh.TriangleIndices.Add(2);
                mesh.TriangleIndices.Add(1);

                //Выбираем цвет для грани
                Color faceColor = faceColors[i % faceColors.Count];
                DiffuseMaterial material = new DiffuseMaterial(new SolidColorBrush(faceColor));
                GeometryModel3D geometry = new GeometryModel3D(mesh, material);

                //Вычисляем нормальный вектор для отсечения задней грани
                Vector3D normal = CalculateNormal(basePoints[i], topPoint, basePoints[next]);

                //Вычисляем центр текущей грани
                Point3D faceCenter = new Point3D(
                    (basePoints[i].X + basePoints[next].X + topPoint.X) / 3,
                    (basePoints[i].Y + basePoints[next].Y + topPoint.Y) / 3,
                    (basePoints[i].Z + basePoints[next].Z + topPoint.Z) / 3);

                if (!IsVisible(normal, camera.Position, faceCenter))
                {
                    geometry.BackMaterial = material; //Отрисовываем заднюю грань, если передняя не видна.
                }
                prismModelGroup.Children.Add(geometry);
            }

            //Создаем нижнюю грань
            MeshGeometry3D bottomMesh = new MeshGeometry3D();

            //Для нижней грани
            for (int i = 0; i < n; i++)
            {
                bottomMesh.Positions.Add(basePoints[i]);
            }

            //Правильный порядок обхода вершин для лицевой стороны (нижняя грань)
            for (int i = 1; i < n - 1; i++)
            {
                bottomMesh.TriangleIndices.Add(0);
                bottomMesh.TriangleIndices.Add(i);
                bottomMesh.TriangleIndices.Add(i + 1);
            }

            //Основание
            Color bottomColor = faceColors[(n + 1) % faceColors.Count];
            DiffuseMaterial bottomMaterial = new DiffuseMaterial(new SolidColorBrush(bottomColor));
            GeometryModel3D bottomGeometry = new GeometryModel3D(bottomMesh, bottomMaterial);

            //Вычисляем нормальный вектор для отсечения задней грани
            Vector3D normalBottom = CalculateNormal(basePoints[0], basePoints[2], basePoints[1]);
            Point3D bottomFaceCenter = new Point3D(centerX, -height / 2, centerZ);

            if (!IsVisible(normalBottom, camera.Position, bottomFaceCenter))
            {
                bottomGeometry.BackMaterial = bottomMaterial; //Отрисовываем заднюю грань, если передняя не видна.
            }

            prismModelGroup.Children.Add(bottomGeometry);
            return prismModelGroup;
        }

        private Matrix3D GetRotationMatrixZ(double angleInDegrees)
        {
            double angleInRadians = angleInDegrees * Math.PI / 180.0;
            Matrix3D rotationMatrix = new Matrix3D();
            rotationMatrix.M11 = Math.Cos(angleInRadians);
            rotationMatrix.M12 = Math.Sin(angleInRadians);
            rotationMatrix.M21 = -Math.Sin(angleInRadians);
            rotationMatrix.M22 = Math.Cos(angleInRadians);
            rotationMatrix.M33 = 1;
            rotationMatrix.M44 = 1;
            return rotationMatrix;
        }

        private Matrix3D GetRotationMatrixX(double angleInDegrees)
        {
            double angleInRadians = angleInDegrees * Math.PI / 180.0;
            Matrix3D rotationMatrix = new Matrix3D();
            rotationMatrix.M11 = 1;
            rotationMatrix.M22 = Math.Cos(angleInRadians);
            rotationMatrix.M23 = Math.Sin(angleInRadians);
            rotationMatrix.M32 = -Math.Sin(angleInRadians);
            rotationMatrix.M33 = Math.Cos(angleInRadians);
            rotationMatrix.M44 = 1;
            return rotationMatrix;
        }

        private Matrix3D GetRotationMatrixY(double angleInDegrees)
        {
            double angleInRadians = angleInDegrees * Math.PI / 180.0;
            Matrix3D rotationMatrix = new Matrix3D();
            rotationMatrix.M11 = Math.Cos(angleInRadians);
            rotationMatrix.M13 = -Math.Sin(angleInRadians);
            rotationMatrix.M22 = 1;
            rotationMatrix.M31 = Math.Sin(angleInRadians);
            rotationMatrix.M33 = Math.Cos(angleInRadians);
            rotationMatrix.M44 = 1;
            return rotationMatrix;
        }

        private Matrix3D GetTranslationMatrix(double x, double y, double z)
        {
            Matrix3D translationMatrix = new Matrix3D();
            translationMatrix.M11 = 1;
            translationMatrix.M22 = 1;
            translationMatrix.M33 = 1;
            translationMatrix.OffsetX = x;
            translationMatrix.OffsetY = y;
            translationMatrix.OffsetZ = z;
            translationMatrix.M44 = 1;
            return translationMatrix;
        }

        private void RotateButton_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(AngleTextBox.Text, out double angle))
            {
                rotationAngle = angle;
            }
            else
            {
                MessageBox.Show("Некорректный угол поворота.");
                return;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Matrix3D combinedRotation = Matrix3D.Identity;
            if (RotatingXCheckBox.IsChecked == true)
            {
                combinedRotation *= GetRotationMatrixX(rotationAngle);
            }
            if (RotatingYCheckBox.IsChecked == true)
            {
                combinedRotation *= GetRotationMatrixY(rotationAngle);
            }
            if (RotatingZCheckBox.IsChecked == true)
            {
                combinedRotation *= GetRotationMatrixZ(rotationAngle);
            }

            //T1 * R * T2 позиционирование афинных преобразований
            Matrix3D T1 = GetTranslationMatrix(centerX, centerY, centerZ); //Матрица переноса к центру объекта
            Matrix3D T2 = GetTranslationMatrix(-centerX, -centerY, -centerZ); //Матрица переноса от центра объекта
            totalTransform = T1 * combinedRotation * T2 * totalTransform;

            //Обновляем трансформацию призмы
            if (prismVisual != null && prismVisual.Content is Model3DGroup prismModelGroup)
            {
                prismModelGroup.Transform = new MatrixTransform3D(totalTransform);
            }
        }

        private void RotatingCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!rotating)
            {
                timer.Start();
                rotating = true;
            }
        }

        private void RotatingCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (RotatingXCheckBox.IsChecked == false && RotatingYCheckBox.IsChecked == false && RotatingZCheckBox.IsChecked == false && rotating)
            {
                timer.Stop();
                rotating = false;
            }
        }

        private void ResetRotationButton_Click(object sender, RoutedEventArgs e)
        {
            totalTransform = Matrix3D.Identity;
            if (prismVisual != null && prismVisual.Content is Model3DGroup prismModelGroup)
            {
                prismModelGroup.Transform = new MatrixTransform3D(totalTransform);
            }
            RotatingXCheckBox.IsChecked = false;
            RotatingYCheckBox.IsChecked = false;
            RotatingZCheckBox.IsChecked = false;
        }

        private Vector3D CalculateNormal(Point3D p1, Point3D p2, Point3D p3)
        {
            Vector3D v1 = p2 - p1;
            Vector3D v2 = p3 - p1;
            return Vector3D.CrossProduct(v1, v2);
        }

        private bool IsVisible(Vector3D normal, Point3D cameraPosition, Point3D faceCenter)
        {
            Vector3D cameraVector = cameraPosition - faceCenter;
            double dotProduct = Vector3D.DotProduct(normal, cameraVector);
            return dotProduct > 0;
        }
    }
}