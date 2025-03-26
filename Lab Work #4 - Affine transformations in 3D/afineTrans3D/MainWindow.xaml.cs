using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using HelixToolkit.Wpf;

namespace AffineTransformations3D
{
    public partial class MainWindow : Window
    {
        private PerspectiveCamera camera; // Камера для просмотра 3D-сцены
        private ModelVisual3D cubeVisual; // Модель куба, которую будем отображать
        private DispatcherTimer timer = new DispatcherTimer(); // Таймер для обновления сцены
        private double rotationAngle = 1.0; // Угол поворота для каждой итерации
        private bool rotating = false; // Флаг для отслеживания состояния анимации
        private double centerX = 0, centerY = 0, centerZ = 0; // Центр вращения
        private Matrix3D totalTransform = Matrix3D.Identity; // Общая трансформация (вращение + перемещение)
        private Color edgeColor = Colors.Black; // Цвет ребер куба

        public MainWindow()
        {
            InitializeComponent();
            InitializeScene(); // Инициализация сцены при старте приложения

            timer.Interval = TimeSpan.FromMilliseconds(16); // Интервал обновления (60 FPS)
            timer.Tick += Timer_Tick; // Подписка на событие таймера
        }

        private void InitializeScene()
        {
            // Создание камеры с позиционированием и настройкой
            camera = new PerspectiveCamera();
            camera.Position = new Point3D(0, 0, 5); // Камера находится на расстоянии 5 от центра
            camera.LookDirection = new Vector3D(0, 0, -1); // Камера смотрит в направлении по оси Z
            camera.UpDirection = new Vector3D(0, 1, 0); // Направление вверх по оси Y
            camera.FieldOfView = 45; // Угол обзора камеры

            // Создание модели куба (первоначальные вершины куба)
            double cubeSize = 0.5;
            Point3D p0 = new Point3D(-cubeSize, -cubeSize, -cubeSize);
            Point3D p1 = new Point3D(cubeSize, -cubeSize, -cubeSize);
            Point3D p2 = new Point3D(cubeSize, -cubeSize, cubeSize);
            Point3D p3 = new Point3D(-cubeSize, -cubeSize, cubeSize);
            Point3D p4 = new Point3D(-cubeSize, cubeSize, -cubeSize);
            Point3D p5 = new Point3D(cubeSize, cubeSize, -cubeSize);
            Point3D p6 = new Point3D(cubeSize, cubeSize, cubeSize);
            Point3D p7 = new Point3D(-cubeSize, cubeSize, cubeSize);

            // Создание материала для ребер куба
            Material edgeMaterial = new DiffuseMaterial(new SolidColorBrush(edgeColor));

            // Группа для всех моделей куба
            Model3DGroup cubeModelGroup = new Model3DGroup();

            // Добавление ребер куба как трубок (линиями)
            double edgeRadius = 0.01; // Радиус трубки, который представляет ребра
            AddTube(cubeModelGroup, new Point3DCollection(new[] { p0, p1 }), edgeRadius, edgeMaterial);
            AddTube(cubeModelGroup, new Point3DCollection(new[] { p1, p2 }), edgeRadius, edgeMaterial);
            AddTube(cubeModelGroup, new Point3DCollection(new[] { p2, p3 }), edgeRadius, edgeMaterial);
            AddTube(cubeModelGroup, new Point3DCollection(new[] { p3, p0 }), edgeRadius, edgeMaterial);

            AddTube(cubeModelGroup, new Point3DCollection(new[] { p4, p5 }), edgeRadius, edgeMaterial);
            AddTube(cubeModelGroup, new Point3DCollection(new[] { p5, p6 }), edgeRadius, edgeMaterial);
            AddTube(cubeModelGroup, new Point3DCollection(new[] { p6, p7 }), edgeRadius, edgeMaterial);
            AddTube(cubeModelGroup, new Point3DCollection(new[] { p7, p4 }), edgeRadius, edgeMaterial);

            AddTube(cubeModelGroup, new Point3DCollection(new[] { p0, p4 }), edgeRadius, edgeMaterial);
            AddTube(cubeModelGroup, new Point3DCollection(new[] { p1, p5 }), edgeRadius, edgeMaterial);
            AddTube(cubeModelGroup, new Point3DCollection(new[] { p2, p6 }), edgeRadius, edgeMaterial);
            AddTube(cubeModelGroup, new Point3DCollection(new[] { p3, p7 }), edgeRadius, edgeMaterial);

            cubeModelGroup.Transform = new MatrixTransform3D(totalTransform); // Применение трансформации

            // Создание визуальной модели для отображения куба
            cubeVisual = new ModelVisual3D();
            cubeVisual.Content = cubeModelGroup;

            // Освещенность сцены
            ModelVisual3D ambientLightVisual = new ModelVisual3D();
            ambientLightVisual.Content = new AmbientLight(Colors.Gray); // Тусклый серый свет

            MyViewport.Camera = camera; // Установка камеры для отображения
            MyViewport.Children.Add(cubeVisual); // Добавление куба на сцену
            MyViewport.Children.Add(ambientLightVisual); // Добавление источника света
        }

        // Метод для добавления трубки (ребра куба)
        private void AddTube(Model3DGroup modelGroup, Point3DCollection path, double radius, Material material)
        {
            var builder = new MeshBuilder(false, false);
            builder.AddTube(path, radius, 8, false, true); // Добавление трубки по заданному пути
            var mesh = builder.ToMesh();
            var geometryModel = new GeometryModel3D(mesh, material); // Модель геометрии с материалом
            modelGroup.Children.Add(geometryModel); // Добавление в группу моделей
        }

        // Матрица поворота вокруг оси Z
        private Matrix3D GetRotationMatrixZ(double angleInDegrees)
        {
            double angleInRadians = angleInDegrees * Math.PI / 180.0; // Преобразуем угол в радианы
            Matrix3D rotationMatrix = new Matrix3D();

            rotationMatrix.M11 = Math.Cos(angleInRadians); // Математика для поворота
            rotationMatrix.M12 = Math.Sin(angleInRadians);
            rotationMatrix.M21 = -Math.Sin(angleInRadians);
            rotationMatrix.M22 = Math.Cos(angleInRadians);
            rotationMatrix.M33 = 1;
            rotationMatrix.M44 = 1;

            return rotationMatrix;
        }

        // Матрица поворота вокруг оси X
        private Matrix3D GetRotationMatrixX(double angleInDegrees)
        {
            double angleInRadians = angleInDegrees * Math.PI / 180.0; // Преобразуем угол в радианы
            Matrix3D rotationMatrix = new Matrix3D();

            rotationMatrix.M11 = 1;
            rotationMatrix.M22 = Math.Cos(angleInRadians);
            rotationMatrix.M23 = Math.Sin(angleInRadians);
            rotationMatrix.M32 = -Math.Sin(angleInRadians);
            rotationMatrix.M33 = Math.Cos(angleInRadians);
            rotationMatrix.M44 = 1;

            return rotationMatrix;
        }

        // Матрица поворота вокруг оси Y
        private Matrix3D GetRotationMatrixY(double angleInDegrees)
        {
            double angleInRadians = angleInDegrees * Math.PI / 180.0; // Преобразуем угол в радианы
            Matrix3D rotationMatrix = new Matrix3D();

            rotationMatrix.M11 = Math.Cos(angleInRadians);
            rotationMatrix.M13 = -Math.Sin(angleInRadians);
            rotationMatrix.M22 = 1;
            rotationMatrix.M31 = Math.Sin(angleInRadians);
            rotationMatrix.M33 = Math.Cos(angleInRadians);
            rotationMatrix.M44 = 1;

            return rotationMatrix;
        }

        // Матрица для перемещения объекта
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

        // Обработчик события нажатия кнопки для изменения угла поворота
        private void RotateButton_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(AngleTextBox.Text, out double angle))
            {
                rotationAngle = angle; // Обновление угла поворота
            }
            else
            {
                MessageBox.Show("Некорректный угол поворота"); // Ошибка при вводе угла
                return;
            }
        }

        // Обработчик события обновления сцены
        private void Timer_Tick(object sender, EventArgs e)
        {
            Matrix3D combinedRotation = Matrix3D.Identity;

            // Применяем повороты по осям, если соответствующие флажки установлены
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

            // Афинные преобразования
            // Применяем перевод (сдвиг) перед и после вращения для правильного положения
            Matrix3D T1 = GetTranslationMatrix(centerX, centerY, centerZ);
            Matrix3D T2 = GetTranslationMatrix(-centerX, -centerY, -centerZ);
            totalTransform = T1 * combinedRotation * T2 * totalTransform;

            // Обновляем трансформацию куба
            if (cubeVisual != null && cubeVisual.Content is Model3DGroup cubeModelGroup)
            {
                cubeModelGroup.Transform = new MatrixTransform3D(totalTransform);
            }
        }

        // Обработчик события начала вращения
        private void RotatingCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!rotating)
            {
                timer.Start(); // Запуск таймера
                rotating = true;
            }
        }

        // Обработчик события остановки вращения
        private void RotatingCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (RotatingXCheckBox.IsChecked == false && RotatingYCheckBox.IsChecked == false && RotatingZCheckBox.IsChecked == false && rotating)
            {
                timer.Stop(); // Остановка таймера
                rotating = false;
            }
        }

        // Обработчик события сброса вращения
        private void ResetRotationButton_Click(object sender, RoutedEventArgs e)
        {
            totalTransform = Matrix3D.Identity; // Сброс трансформации

            if (cubeVisual != null && cubeVisual.Content is Model3DGroup cubeModelGroup)
            {
                cubeModelGroup.Transform = new MatrixTransform3D(totalTransform); // Применение сброшенной трансформации
            }

            RotatingXCheckBox.IsChecked = false;
            RotatingYCheckBox.IsChecked = false;
            RotatingZCheckBox.IsChecked = false;
        }
    }
}
