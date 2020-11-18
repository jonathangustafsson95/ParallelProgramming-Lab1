using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Mandelbrot;
using Utilities;

namespace MeasurementApp.ViewModels
{
    class MeasureMandelbrot : ViewModelBase
    {
        public MandelbrotBase[] AvailableMandelbrotAlgorithms {
            get { return availableMandelbrotAlgorithms; }
        }

        public double LowerX {
            get {
                return lowerX;
            }
            set {
                lowerX = value;
                OnPropertyChanged();
            }
        }
        public double UpperX {
            get {
                return upperX;
            }
            set {
                upperX = value;
                OnPropertyChanged();
            }
        }
        public double LowerY {
            get {
                return lowerY;
            }
            set {
                lowerY = value;
                OnPropertyChanged();
            }
        }
        public double UpperY {
            get {
                return upperY;
            }
            set {
                upperY = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand<object> RunCommand {
            get; private set;
        }

        public bool RunIsEnabled {
            get {
                return runIsEnabled;
            }
            private set {
                runIsEnabled = value;
                OnPropertyChanged();
            }
        }
        
        public string ResultLog {
            get {
                return resultLog;
            }
            set {
                resultLog = value;
                OnPropertyChanged();
            }
        }

        public BitmapSource MandelbrotImage {
            get {
                return mandelbrotImage;
            }
            private set {
                mandelbrotImage = value;
                OnPropertyChanged();
            }
        }

        public MeasureMandelbrot()
        {
            RunCommand = new RelayCommand<object>(x => ToRun(), null);
            RunIsEnabled = true;
            LowerX = -1.0;
            UpperX = 1.0;
            LowerY = -1.0;
            UpperY = 1.0;
        }

        private void ToRun()
        {
            RunIsEnabled = false;
            Task[] start = new Task[AvailableMandelbrotAlgorithms.Length];
            Task[] end = new Task[AvailableMandelbrotAlgorithms.Length];
            for (int i = 0; i < AvailableMandelbrotAlgorithms.Length; i++) {
                MandelbrotBase m = AvailableMandelbrotAlgorithms[i];
                m.LowerX = LowerX;
                m.UpperX = UpperX;
                m.LowerY = LowerY;
                m.UpperY = UpperY;
                start[i] = new Task(() => {});
                end[i] = start[i].ContinueWith(t => {
                        // Render the initialization to the UI.
                        ResultLog += "Algorithm " + m.Name + ": Started with (" +
                            LowerX + ", " + UpperX + ")x(" + LowerY + ", " + UpperY +
                            ")." + Environment.NewLine;
                    }, TaskScheduler.FromCurrentSynchronizationContext()).ContinueWith(t => {
                        // Run in the background a long computation which generates a result.
                        return Run(m);
                    }).ContinueWith(t => {
                        // Render the result on the UI.
                        ResultLog += "Algorithm " + m.Name + ": " +
                            t.Result.Item1.ToString() + " wall clock sec. " +
                            t.Result.Item2.ToString() + " processor user time sec." +
                            Environment.NewLine;

                        // Ugly conversion to image.
                        byte[] image = new byte[m.Image.GetLength(0) * m.Image.GetLength(1)];
                        for (int x = 0; x < m.Image.GetLength(0); x++) {
                            for(int y = 0; y < m.Image.GetLength(1); y++) {
                                image[x + (m.Image.GetLength(1) - y - 1)*m.Image.GetLength(1)] = (byte)m.Image[x, y];
                            }
                        }
                        MandelbrotImage =
                            BitmapSource.Create(m.Image.GetLength(0),
                                                m.Image.GetLength(1),
                                                96, 96,
                                                PixelFormats.Gray8,
                                                null,
                                                image,
                                                m.Image.GetLength(0));
                    }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            Task beginning = new Task(() => {
                    for (int i = 0; i < start.Length; i++) {
                        start[i].RunSynchronously();
                        end[i].Wait();
                    }
                });
            beginning.ContinueWith(t => {
                // Enable the start button.
                RunIsEnabled = true;
            }, TaskScheduler.FromCurrentSynchronizationContext());

            beginning.Start();
        }

        private static Tuple<double, double> Run(MandelbrotBase mandelbrot)
        {
            Stopwatch wallClock = new Stopwatch();
            ProcessUserTime processUserTime = new ProcessUserTime();

            // Measure the Mandelbrot computation.
            wallClock.Restart();
            processUserTime.Restart();
            mandelbrot.Compute();
            processUserTime.Stop();
            wallClock.Stop();
            return
                new Tuple<double, double>(wallClock.Elapsed.TotalSeconds,
                                          processUserTime.ElapsedTotalSeconds);
        }


        private readonly MandelbrotBase[] availableMandelbrotAlgorithms = {
                new MandelbrotSingleThread(2048, 2048)
            };
        private bool runIsEnabled;
        private string resultLog;
        private double lowerX, upperX, lowerY, upperY;
        private BitmapSource mandelbrotImage;
    }
}
