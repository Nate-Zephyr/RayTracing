using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _3DRayCastingNew
{
    public class DoubleBufferedForm : Form
    {
        public DoubleBufferedForm()
        {
            DoubleBuffered = true;
        }
        public void SetName(string name)
        {
            Text = name;
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Game.Reset();
        }
    }
    public class Game
    {
        #region Подключение библиотек

        // Получение состояния клавиши
        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int key);
        #endregion

        #region Объявление глобальных переменных

        public static Random Rand = new Random();

        public static int WindowWidthScaled;
        public static int WindowHeightScaled;
        public static double Scale = 0.1;

        public static double AspectRatio;

        public static DoubleBufferedForm GameWindow = new DoubleBufferedForm();

        public static bool isReseting = false;

        public static Vec3 Light = Vec.Norm(new Vec3(0, 0, 0));
        public static double Ambient = 0.1;
        public static int TraceLimit = 4;

        public static bool RotateLight = true;
        public static bool CastShadows = true;

        public static string SystemInfo = "";

        public static Vec3[] Output3D;
        #endregion

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();

            GameWindow.Size = new Size(1280, 720);
            GameWindow.WindowState = FormWindowState.Maximized;
            GameWindow.BackgroundImageLayout = ImageLayout.Stretch;
            GameWindow.Show();

            WindowWidthScaled = (int)(GameWindow.Size.Width * Scale);
            WindowHeightScaled = (int)(GameWindow.Size.Height * Scale);

            AspectRatio = WindowWidthScaled / (double)WindowHeightScaled;

            Output3D = new Vec3[WindowWidthScaled * WindowHeightScaled];

            Camera MainCamera = new Camera(new Vec3(0, -10, -1), new Vec2(0, 0), 0.75);

            List<GameObject> GameObjects = new List<GameObject>();
            GameObjects.Add(new GameObject(0, "Box", new Vec3(0, 0, 1), new Vec3(100, 100, 0.1), new Vec3(0.20, 0.20, 0.25), 1, 0.975, 0.025, 0, 0));

            GameObjects.Add(new GameObject(GameObjects.Count, "Box", new Vec3(-4.9, 0, -1), new Vec3(0.1, 5, 2), new Vec3(0, 0, 1), 1, 0.05, 0.25, 0.7, 1.5));
            GameObjects.Add(new GameObject(GameObjects.Count, "Box", new Vec3(4.9, 0, -1), new Vec3(0.1, 5, 2), new Vec3(0, 0, 1), 1, 0.05, 0.25, 0.7, 1.5));

            GameObjects.Add(new GameObject(GameObjects.Count, "Box", new Vec3(-2.9, 4.9, -1), new Vec3(1.9, 0.1, 2), new Vec3(0, 0, 1), 1, 0.05, 0.25, 0.7, 1.5));
            GameObjects.Add(new GameObject(GameObjects.Count, "Box", new Vec3(2.9, 4.9, -1), new Vec3(1.9, 0.1, 2), new Vec3(0, 0, 1), 1, 0.05, 0.25, 0.7, 1.5));

            GameObjects.Add(new GameObject(GameObjects.Count, "Box", new Vec3(-2.9, -4.9, -1), new Vec3(1.9, 0.1, 2), new Vec3(0, 0, 1), 1, 0.05, 0.25, 0.7, 1.5));
            GameObjects.Add(new GameObject(GameObjects.Count, "Box", new Vec3(2.9, -4.9, -1), new Vec3(1.9, 0.1, 2), new Vec3(0, 0, 1), 1, 0.05, 0.25, 0.7, 1.5));

            GameObjects.Add(new GameObject(GameObjects.Count, "Sphere", new Vec3(0, 0, -2), 1, new Vec3(1, 0, 0), 1, 0.25, 0.5, 0.25, 1.5));

            Thread Thread_LightRotation = new Thread(LightRotation);
            Thread Thread_Render3D = new Thread(() => Render3D(MainCamera, GameObjects));

            Thread Thread_CameraControlMouse = new Thread(() => CameraControlMouse(MainCamera));
            Thread Thread_CameraControl = new Thread(() => CameraControl(MainCamera));
            Thread Thread_OtherControls = new Thread(() => OtherControls(MainCamera));

            Thread_LightRotation.Priority = ThreadPriority.Lowest;
            Thread_Render3D.Priority = ThreadPriority.Highest;

            Thread_CameraControlMouse.Priority = ThreadPriority.AboveNormal;
            Thread_CameraControl.Priority = ThreadPriority.Normal;
            Thread_OtherControls.Priority = ThreadPriority.Lowest;

            Thread_LightRotation.Start();
            Thread_Render3D.Start();

            Thread_CameraControlMouse.Start();
            Thread_CameraControl.Start();
            Thread_OtherControls.Start();
        }
        public static void Reset()
        {
            try
            {
                isReseting = true;
                Thread.Sleep(300);

                WindowWidthScaled = (int)(GameWindow.Size.Width * Scale);
                WindowHeightScaled = (int)(GameWindow.Size.Height * Scale);

                AspectRatio = WindowWidthScaled / (double)WindowHeightScaled;

                Output3D = new Vec3[WindowWidthScaled * WindowHeightScaled];

                isReseting = false;
            } 
            catch (Exception ex) { try { File.AppendAllText("log.txt", DateTime.Now + "\nОшибка при перестройке окна:\n" + ex.ToString() + "\n\n\n", Encoding.UTF8); } catch { } }
        }
        public static void DrawFrame()
        {
            if (isReseting)
                return;

            try
            {
                Bitmap temp = new Bitmap(WindowWidthScaled, WindowHeightScaled);
                Vec3[] bufferOutput3D = (Vec3[])Output3D.Clone();

                for (int y = 0; y < WindowHeightScaled; y++)
                    for (int x = 0; x < WindowWidthScaled; x++)
                        temp.SetPixel(x, y, Color.FromArgb((int)bufferOutput3D[x + y * WindowWidthScaled].x, (int)bufferOutput3D[x + y * WindowWidthScaled].y, (int)bufferOutput3D[x + y * WindowWidthScaled].z));

                GameWindow.BackgroundImage = temp;
                GameWindow.SetName(SystemInfo);
            }
            catch (Exception ex) { try { File.AppendAllText("log.txt", DateTime.Now + "\nОшибка при обновлении экрана:\n" + ex.ToString() + "\n\n\n", Encoding.UTF8); } catch { } }
        }
        public static async void Render3D(Camera MainCamera, List<GameObject> GameObjects)
        {
            int fps = 0;
            int frameCounter = 0;
            DateTime frameStartingTime = DateTime.Now;

            Output3D = new Vec3[WindowWidthScaled * WindowHeightScaled];

            while (true)
            {
                if (isReseting)
                    continue;

                try
                {
                    Vec3 cameraPosition = MainCamera.Position;
                    Mat3 cameraRotation = Vec.Rotate3(MainCamera.Rotation);
                    double fov = MainCamera.FOV;

                    Vec3 light = Light;

                    var rayCastingTasks = new List<Task>();

                    for (int i = 0; i < WindowHeightScaled; i++)
                    {
                        for (int j = 0; j < WindowWidthScaled; j++)
                        {
                            int i1 = i;
                            int j1 = j;

                            //rayCastingTasks.Add(Task.Run(() => CastRay(i1, j1, GameObjects, tempLight, tempCameraPosition, tempCameraRotation, tempFOV)));
                            rayCastingTasks.Add(Task.Run(() => TraceRay(i1, j1, GameObjects, light, cameraPosition, cameraRotation, fov)));

                            // Надо добавить переключание между синхронным и асинхронным рендером
                            //TraceRay(i1, j1, GameObjects, tempLight, tempCameraPosition, tempCameraRotation, tempFOV);
                        }
                    }

                    await Task.WhenAll(rayCastingTasks);

                    // Отображение инфо
                    SystemInfo = $"FPS:{fps}".PadRight(3) + $"   {WindowWidthScaled}x{WindowHeightScaled} ({Scale} Scale)" + $"   X:{MainCamera.Position.x:f3} Y:{MainCamera.Position.y:f3} Z:{MainCamera.Position.z:f3}"
                                                       + $"   H:{MainCamera.Rotation.x:f3} V:{MainCamera.Rotation.y:f3}" + $"   FOV:{MainCamera.FOV}";

                    // Рассчёт FPS
                    if ((DateTime.Now - frameStartingTime).TotalSeconds >= 0.5)
                    {
                        if (frameCounter > 0)
                            fps = frameCounter * 2;
                        else
                            fps = 0;

                        frameCounter = 0;
                        frameStartingTime = DateTime.Now;
                    }
                    else frameCounter++;
                }
                catch (Exception ex) { try { File.AppendAllText("log.txt", DateTime.Now + "\nОшибка при рендере 3D:\n" + ex.ToString() + "\n\n\n", Encoding.UTF8); } catch { } }

                Task.Run(DrawFrame);
            }
        }
        public static void TraceRay(int i, int j, List<GameObject> GameObjects, Vec3 light, Vec3 cameraPosition, Mat3 cameraRotation, double fov)
        {
            Vec2 uv = new Vec2(j, i) / new Vec2(WindowWidthScaled, WindowHeightScaled) - 0.5;
            uv.x *= AspectRatio;
            Vec3 rayDirection = Vec.Norm(new Vec3(uv.x, fov, uv.y)) * cameraRotation;

            Vec3 color = CastRay(false, false, TraceLimit, -1, GameObjects, light, cameraPosition, rayDirection);

            /*if (color == new Vec3(0)) // Вывод "скайбокса"
                Output3D[i * WindowWidthScaled + j] = CalculateColorRGB(new Vec3(0.25, 0.5, 1), 1 + Math.Pow(Math.Max(0, Vec.Dot(rayDirection, tempLight)), 512));
            else // Вывод пересечения*/
                Output3D[i * WindowWidthScaled + j] = color;
        }
        public static Vec3 CastRay(bool refracted, bool shadowRay, int traceLimit, int id, List<GameObject> GameObjects, Vec3 light, Vec3 RO, Vec3 RD)
        {
            double minIntersection = double.MaxValue;
            Vec3 normal = new Vec3(0);
            double brightness = 0;
            double diffusion = 0;
            double reflection = 0;
            double refraction = 0;
            double refractionRate = 1;
            Vec3 color = new Vec3(0);

            // Поиск ближайшего пересечения
            foreach (GameObject gameObject in GameObjects)
            {
                if (gameObject.ID == id)
                    continue;

                switch (gameObject.Shape)
                {
                    case "Triangle":
                        {
                            Vec3 tempNormal = new Vec3(0);

                            gameObject.Intersection = Vec.triIntersection(RO, RD, gameObject.Vert1, gameObject.Vert2, gameObject.Vert3, ref tempNormal);
                            if (gameObject.Intersection > 0 && gameObject.Intersection < minIntersection)
                            {
                                id = gameObject.ID;
                                minIntersection = gameObject.Intersection;
                                brightness = gameObject.Brightness;
                                diffusion = gameObject.Diffusion;
                                reflection = gameObject.Reflection;
                                refraction = gameObject.Refraction;
                                refractionRate = gameObject.RefractionRate;
                                normal = tempNormal;
                                color = gameObject.Color;
                            }
                            break;
                        }
                    case "Sphere":
                        {
                            Vec3 tempNormal = new Vec3(0);

                            if (!refracted)
                                gameObject.Intersection = Vec.sphIntersection(RO, RD, gameObject.Position, gameObject.Radius, ref tempNormal).x;
                            else
                                gameObject.Intersection = Vec.sphIntersection(RO, RD, gameObject.Position, gameObject.Radius, ref tempNormal).y;

                            if (gameObject.Intersection > 0 && gameObject.Intersection < minIntersection)
                            {
                                id = gameObject.ID;
                                minIntersection = gameObject.Intersection;
                                brightness = gameObject.Brightness;
                                diffusion = gameObject.Diffusion;
                                reflection = gameObject.Reflection;
                                refraction = gameObject.Refraction;
                                refractionRate = gameObject.RefractionRate;
                                normal = tempNormal;
                                color = gameObject.Color;
                            }
                            break;
                        }
                    case "Box":
                        {
                            Vec3 tempNormal = new Vec3(0);

                            if (!refracted)
                                gameObject.Intersection = Vec.boxIntersection(RO, RD, gameObject.Position, gameObject.Size, ref tempNormal).x;
                            else
                                gameObject.Intersection = Vec.boxIntersection(RO, RD, gameObject.Position, gameObject.Size, ref tempNormal).y;

                            if (gameObject.Intersection > 0 && gameObject.Intersection < minIntersection)
                            {
                                id = gameObject.ID;
                                minIntersection = gameObject.Intersection;
                                brightness = gameObject.Brightness;
                                diffusion = gameObject.Diffusion;
                                reflection = gameObject.Reflection;
                                refraction = gameObject.Refraction;
                                refractionRate = gameObject.RefractionRate;
                                normal = tempNormal;
                                color = gameObject.Color;
                            }
                            break;
                        }
                }
            }

            if (shadowRay)
                return color;

            if (refracted)
                return CastRay(false, false, traceLimit, id, GameObjects, light, RO + RD * minIntersection, Refract(normal, RD, refractionRate, 1));

            double diffuse;

            if (minIntersection < double.MaxValue)
            {
                // Поиск затеняющего объекта, если его нет, осветить объект соглнасно нормали
                if (Vec.Dot(normal, light) < 0 || CastRay(false, true, -1, id, GameObjects, -light, RO + RD * minIntersection /*- light * 0.0001*/, light) != new Vec3(0)) //traceLimit == TraceLimit &&
                    diffuse = 0;
                else
                    diffuse = Math.Max(0, Vec.Dot(normal, light) * brightness) + Math.Pow(Math.Max(0, Vec.Dot(RD - normal * Vec.Dot(normal, RD) * 2, light)), 32) * reflection; // + блик (отражение источника света)

                if (diffuse + Ambient <= 1)
                    diffuse += Ambient;
            }
            else
                return CalculateColorRGB(new Vec3(0.25, 0.5, 1), -light.z * 0.5 + 0.5 + Math.Pow(Math.Max(0, Vec.Dot(RD, light)), 512)); // Небо + Солнце

            if (traceLimit > 0)
                return (diffusion == 0 ? new Vec3(0) : diffusion * CalculateColorRGB(color, diffuse)) // Поглащение
                     + (reflection == 0 ? new Vec3(0) : reflection * CastRay(false, false, traceLimit - 1, id, GameObjects, Light, RO + RD * minIntersection, RD - 2 * Vec.Dot(normal, RD) * normal)) // Отражение
                     + (refraction == 0 ? new Vec3(0) : refraction * CastRay(true, false, traceLimit - 1, -1, GameObjects, Light, RO + RD * minIntersection, Refract(normal, RD, 1, refractionRate))); // Преломление
            else                                                                                                                             // После того, как луч проходит через объект, его нужно приломить ещё раз, а только потом искать цвет
                return CalculateColorRGB(color, diffuse); // Полное поглащение
        }
        public static Vec3 Refract(Vec3 normal, Vec3 RD, double n1, double n2)
        {
            double n = n1 / n2;
            double cosI = -Vec.Dot(normal, RD);
            double sinT2 = n * n * (1 - cosI * cosI);
            if (sinT2 > 1)
                return new Vec3(RD - 2 * Vec.Dot(-normal, RD) * -normal); // TIR

            double cosT = Math.Sqrt(1 - sinT2);
            return n * RD + (n * cosI - cosT) * normal;
        }
        /*public static void CastRay(int i, int j, List<GameObject> GameObjects, Vec3 tempLight, Vec3 tempCameraPosition, Mat3 tempCameraRotation, double tempFOV)
        {
            Vec2 uv = new Vec2(j, i) / new Vec2(WindowWidthScaled, WindowHeightScaled) - 0.5;
            uv.x *= AspectRatio;
            uv += 0.0001; // + 0.0001 чтобы не было артефактов из-за идельного угла камеры (прямо на сцену)
            Vec3 rayDirection = Vec.Norm(new Vec3(uv.x, tempFOV, uv.y))* tempCameraRotation;

            int tempID = -1;
            double minIntersection = double.MaxValue;
            Vec3 normal = new Vec3(0);
            double diffuse = 0;
            double specular = 0;
            Vec3 color = new Vec3(0);

            // Поиск ближайшего пересечения
            foreach (GameObject gameObject in GameObjects)
            {
                switch (gameObject.Shape)
                {
                    case "Triangle":
                        {
                            Vec3 tempNormal = new Vec3(0);

                            gameObject.Intersection = Vec.triIntersection(tempCameraPosition, rayDirection, gameObject.Vert1, gameObject.Vert2, gameObject.Vert3, ref tempNormal);
                            if (gameObject.Intersection > 0 && gameObject.Intersection < minIntersection)
                            {
                                tempID = gameObject.ID;
                                minIntersection = gameObject.Intersection;
                                normal = tempNormal;
                                diffuse = Math.Max(0, Vec.Dot(normal, tempLight) * gameObject.Brightness);
                                specular = Math.Pow(Math.Max(0, Vec.Dot(rayDirection - normal * Vec.Dot(normal, rayDirection) * 2, tempLight)), 32) * gameObject.Glossiness;
                                color = gameObject.Color;
                            }
                            break;
                        }
                    case "Plane":
                        {
                            if (tempCameraPosition.z >= gameObject.Position.z)
                                break;

                            Vec3 tempNormal = new Vec3(0);

                            gameObject.Intersection = Vec.boxIntersection(tempCameraPosition, rayDirection, gameObject.Position, gameObject.Size, ref tempNormal);
                            if (gameObject.Intersection > 0 && gameObject.Intersection < minIntersection)
                            {
                                tempID = gameObject.ID;
                                minIntersection = gameObject.Intersection;
                                normal = tempNormal;
                                diffuse = Math.Max(0, Vec.Dot(normal, tempLight) * gameObject.Brightness);
                                specular = Math.Pow(Math.Max(0, Vec.Dot(rayDirection - normal * Vec.Dot(normal, rayDirection) * 2, tempLight)), 32) * gameObject.Glossiness;
                                color = gameObject.Color;
                            }
                            break;
                        }
                    case "Sphere":
                        {
                            Vec3 tempNormal = new Vec3(0);

                            gameObject.Intersection = Vec.sphIntersection(tempCameraPosition, rayDirection, gameObject.Position, gameObject.Radius, ref tempNormal);
                            if (gameObject.Intersection > 0 && gameObject.Intersection < minIntersection)
                            {
                                tempID = gameObject.ID;
                                minIntersection = gameObject.Intersection;
                                normal = tempNormal;
                                diffuse = Math.Max(0, Vec.Dot(normal, tempLight) * gameObject.Brightness);
                                specular = Math.Pow(Math.Max(0, Vec.Dot(rayDirection - normal * Vec.Dot(normal, rayDirection) * 2, tempLight)), 32) * gameObject.Glossiness;
                                color = gameObject.Color;
                            }
                            break;
                        }
                    case "Box":
                        {
                            Vec3 tempNormal = new Vec3(0);

                            gameObject.Intersection = Vec.boxIntersection(tempCameraPosition, rayDirection, gameObject.Position, gameObject.Size, ref tempNormal);
                            if (gameObject.Intersection > 0 && gameObject.Intersection < minIntersection)
                            {
                                tempID = gameObject.ID;
                                minIntersection = gameObject.Intersection;
                                normal = tempNormal;
                                diffuse = Math.Max(0, Vec.Dot(normal, tempLight) * gameObject.Brightness);
                                specular = Math.Pow(Math.Max(0, Vec.Dot(rayDirection - normal * Vec.Dot(normal, rayDirection) * 2, tempLight)), 32) * gameObject.Glossiness;
                                color = gameObject.Color;
                            }
                            break;
                        }
                }
            }

            if (!CastShadows || minIntersection == double.MaxValue)
                goto Skip;

            // Поиск любого пересечения в направлении к источнику света (Нахождение тени)
            if (Vec.Dot(normal, tempLight) > 0)
            {
                Vec3 tempRO = tempCameraPosition + rayDirection * minIntersection - tempLight * 0.0001;
                foreach (GameObject gameObject in GameObjects)
                {
                    if (gameObject.ID == tempID)
                        continue;

                    switch (gameObject.Shape)
                    {
                        case "Triangle":
                            {
                                foreach (int id in gameObject.MergedID)
                                    if (id == tempID)
                                        goto NoSelfShadow;

                                Vec3 tempNormal = new Vec3(0);

                                double intersection = Vec.triIntersection(tempRO, tempLight, gameObject.Vert1, gameObject.Vert2, gameObject.Vert3, ref tempNormal);
                                if (intersection > -1)
                                {
                                    diffuse = 0;
                                    specular = 0;
                                    goto Skip;
                                }
                            NoSelfShadow:
                                break;
                            }
                        case "Sphere":
                            {
                                Vec3 tempNormal = new Vec3(0);

                                double intersection = Vec.sphIntersection(tempRO, -tempLight, gameObject.Position, gameObject.Radius, ref tempNormal);
                                if (intersection < -1)
                                {
                                    diffuse = 0;
                                    specular = 0;
                                    goto Skip;
                                }
                                break;
                            }
                        case "Box":
                            {
                                Vec3 tempNormal = new Vec3(0);

                                double intersection = Vec.boxIntersection(tempRO, tempLight, gameObject.Position, gameObject.Size, ref tempNormal);
                                if (intersection > -1)
                                {
                                    diffuse = 0;
                                    specular = 0;
                                    goto Skip;
                                }
                                break;
                            }
                        case "Plane":
                            {
                                if (tempCameraPosition.z >= gameObject.Position.z)
                                    break;

                                Vec3 tempNormal = new Vec3(0);

                                double intersection = Vec.boxIntersection(tempRO, tempLight, gameObject.Position, gameObject.Size, ref tempNormal);
                                if (intersection > -1)
                                {
                                    diffuse = 0;
                                    specular = 0;
                                    goto Skip;
                                }
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }
            }

        Skip:

            if (minIntersection == double.MaxValue) // Вывод "скайбокса"
            {
                Output3D[i * WindowWidthScaled + j] = CalculateColorRGB(new Vec3(0.25, 0.5, 1), 1 + Math.Pow(Math.Max(0, Vec.Dot(rayDirection, tempLight)), 512));
            }
            else // Вывод пересечения
            {
                if (minIntersection < double.MaxValue)
                {
                    diffuse += specular;
                    if (diffuse + Ambient <= 1)
                        diffuse += Ambient;
                }
                Output3D[i * WindowWidthScaled + j] = CalculateColorRGB(color, diffuse);
            }
        }*/
        public static Vec3 CalculateColorRGB(Vec3 Color, double Brightness)
        {
            double extraBrightness;

            if (Brightness < 0)
            {
                Brightness = 0;
            }
            if (Brightness > 1)
            {
                extraBrightness = Brightness - 1;
                Color += extraBrightness;

                Brightness = 1;
            }

            Color *= Brightness * 255;

            if (Color.x > 255) Color.x = 255;
            if (Color.y > 255) Color.y = 255;
            if (Color.z > 255) Color.z = 255;

            return new Vec3(Color.x, Color.y, Color.z);
        }
        public static void LightRotation()
        {
            while (true)
            {
                DateTime startingTime = DateTime.Now;

                if (!RotateLight)
                    goto Skip;
                Light = Vec.Norm(new Vec3(Math.Cos((DateTime.Now.Second + DateTime.Now.Millisecond / 1000d) * 6 * Math.PI / 180 * 3),
                                          Math.Sin((DateTime.Now.Second + DateTime.Now.Millisecond / 1000d) * 6 * Math.PI / 180 * 3),
                                          -Math.Sin((DateTime.Now.Second + DateTime.Now.Millisecond / 1000d) * 6 * Math.PI / 180 * 3) * 0.5 - 0.5 - 0.75));
            Skip:
                DateTime finishingTime = DateTime.Now;
                if ((finishingTime - startingTime).TotalMilliseconds < 10)
                    Thread.Sleep((int)(10 - (finishingTime - startingTime).TotalMilliseconds));
            }
        }
        public static void CameraControlMouse(Camera MainCamera)
        {
            Point centre = new Point(200, 100);
            Cursor.Position = centre;

            Vec2 mouseMove;

            double cameraRotationSpeed = 0.001;

            while (true)
            {
                DateTime startingTime = DateTime.Now;

                mouseMove = new Vec2(centre.X - Cursor.Position.X, centre.Y - Cursor.Position.Y);

                MainCamera.Rotation -= mouseMove * cameraRotationSpeed;

                Cursor.Position = centre;

                DateTime finishingTime = DateTime.Now;
                if ((finishingTime - startingTime).TotalMilliseconds < 10)
                    Thread.Sleep((int)(10 - (finishingTime - startingTime).TotalMilliseconds));
            }
        }
        public static void CameraControl(Camera MainCamera)
        {
            double cameraSpeed = 0.1;
            double cameraRotationSpeed = 0.01;

            while (true)
            {
                DateTime startingTime = DateTime.Now;

                // Движение
                if (GetAsyncKeyState(87) != 0) // W
                {
                    MainCamera.Position.x += Math.Sin(MainCamera.Rotation.x) * cameraSpeed;
                    MainCamera.Position.y += Math.Cos(MainCamera.Rotation.x) * cameraSpeed;
                }
                if (GetAsyncKeyState(83) != 0) // S
                {
                    MainCamera.Position.x -= Math.Sin(MainCamera.Rotation.x) * cameraSpeed;
                    MainCamera.Position.y -= Math.Cos(MainCamera.Rotation.x) * cameraSpeed;
                }
                if (GetAsyncKeyState(65) != 0) // A
                {
                    MainCamera.Position.x -= Math.Sin(MainCamera.Rotation.x + Math.PI / 2d) * cameraSpeed;
                    MainCamera.Position.y -= Math.Cos(MainCamera.Rotation.x + Math.PI / 2d) * cameraSpeed;
                }
                if (GetAsyncKeyState(68) != 0) // D
                {
                    MainCamera.Position.x += Math.Sin(MainCamera.Rotation.x + Math.PI / 2d) * cameraSpeed;
                    MainCamera.Position.y += Math.Cos(MainCamera.Rotation.x + Math.PI / 2d) * cameraSpeed;
                }
                if (GetAsyncKeyState(32) != 0) // Пробел
                {
                    MainCamera.Position.z -= cameraSpeed;
                }
                if (GetAsyncKeyState(17) != 0) // Контрол
                {
                    MainCamera.Position.z += cameraSpeed;
                }

                // Вращение
                if (GetAsyncKeyState(38) != 0) // ^
                {
                    MainCamera.Rotation.y -= cameraRotationSpeed;
                }
                if (GetAsyncKeyState(40) != 0) // v
                {
                    MainCamera.Rotation.y += cameraRotationSpeed;
                }
                if (GetAsyncKeyState(37) != 0) // <
                {
                    MainCamera.Rotation.x -= cameraRotationSpeed;
                }
                if (GetAsyncKeyState(39) != 0) // >
                {
                    MainCamera.Rotation.x += cameraRotationSpeed;
                }

                DateTime finishingTime = DateTime.Now;
                if ((finishingTime - startingTime).TotalMilliseconds < 10)
                    Thread.Sleep((int)(10 - (finishingTime - startingTime).TotalMilliseconds));
            }
        }
        public static void OtherControls(Camera MainCamera)
        {
            while (true)
            {
                DateTime startingTime = DateTime.Now;

                // Выход
                if (GetAsyncKeyState(27) != 0) // Esc
                {
                    Environment.Exit(0);
                }

                // Вращение источника света Вкл/Выкл
                if (GetAsyncKeyState(112) != 0) // F1
                {
                    RotateLight = !RotateLight;
                    Thread.Sleep(300);
                }

                // Поле зрения
                if (GetAsyncKeyState(113) != 0) // F2
                {
                    MainCamera.FOV += 0.01;
                    Thread.Sleep(30);
                }
                if (GetAsyncKeyState(114) != 0) // F3
                {
                    MainCamera.FOV -= 0.01;
                    Thread.Sleep(30);
                }

                // Тени Вкл/Выкл
                if (GetAsyncKeyState(115) != 0) // F4
                {
                    CastShadows = !CastShadows;
                    Thread.Sleep(300);
                }

                // Разрешение
                if ((GetAsyncKeyState(187) != 0 || GetAsyncKeyState(107) != 0) && !isReseting) // +
                {
                    if (Scale < 2)
                    {
                        Scale += 0.05;
                        Reset();
                    }
                }
                if ((GetAsyncKeyState(189) != 0 || GetAsyncKeyState(108) != 0) && !isReseting) // -
                {
                    if (Scale > 0.05)
                    {
                        Scale -= 0.05;
                        Reset();
                    }
                }

                DateTime finishingTime = DateTime.Now;
                if ((finishingTime - startingTime).TotalMilliseconds < 10)
                    Thread.Sleep((int)(10 - (finishingTime - startingTime).TotalMilliseconds));
            }
        }
    }
}
