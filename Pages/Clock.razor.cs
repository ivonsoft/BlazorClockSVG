using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using System.Threading;

namespace BlazorClockSVG.Shared
{
    public partial class ClockModel : ComponentBase, IDisposable
    {
        private event EventHandler<ThresholdReachedEventArgs> ThresholdReached;
        protected string transform_second;
        private string transform_minute;
        private CancellationTokenSource TokenSource;
        private Task ClockTask;

        protected RenderFragment minutehandShadow;
        protected RenderFragment secondhandShadow;
        protected RenderFragment hourhandShadow;

        protected RenderFragment electronicDisplayer;
        protected RenderFragment hourhand;
        //private RenderFragment secondhand;
        protected RenderFragment minutehand;
        private string transform_hour;
        private int ThresholdcounterTotal;
        private int Thresholdcounter;

        [Parameter] public string viewBox { set; get; }
        [Parameter] public int SVGwidth { set; get; }
        [Parameter] public int SVGheight { set; get; }
        [Parameter] public string sec_color { set; get; }
        [Parameter] public string min_color { set; get; }
        [Parameter] public string hr_color { set; get; }
        [Parameter] public int sec_radius { set; get; }
        [Parameter] public int min_radius { set; get; }
        [Parameter] public int hr_radius { set; get; }
        [Parameter] public RenderFragment ChildContent { set; get; }
        private const int diff_radius = 2;
        protected double sec_circumference;
        protected double sec_circ_offset;
        protected double min_circumference;
        protected double min_circ_offset;
        protected double hr_circumference;
        protected double hr_circ_offset;

        private double sec_circ_tick;
        private double min_circ_tick;
        private double hr_circ_tick;
        private strSVG SVG;
        struct strSVG
        {
            public int width;
            public int height;
            public int promien;

        }
        /* long x;
         long y;*/
        struct options
        {
            public double value;
            public int radius;
            public string klasa;
            public string transform;
        }
        protected struct metrics
        {
            public int number;
            public string Name;
            public string klasa;
        }
        private Boolean running = false;
        private string canvasFont = "20px Digital-7";
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            sec_color = (sec_color == null) ? "rgba(241, 56, 51, 0.9)" : sec_color;
            min_color = "rgba(88, 251, 88, 0.9)";
            hr_color = "rgba(60, 61, 231, 0.9)";
        }

        [Inject] protected IJSRuntime JSRuntime { get; set; }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                SVG = new strSVG();
                ThresholdcounterTotal = 10;
                //  Console.WriteLine($"{windowSize.Height}, {windowSize.Width}");
                hourhand = BuildSVG_hourhand();
                minutehand = BuildSVG_minutehand();
                // secondhand is defined in razor file
                viewBox = "0 0 100 100";
                SVGwidth = 200;
                SVG.width = SVGwidth;
                SVGheight = 200;
                SVG.height = SVGheight;
                SVG.promien = 50;
                sec_radius = 48;
                min_radius = sec_radius - diff_radius;
                hr_radius = min_radius - diff_radius;
                sec_circumference = Math.Round(2 * sec_radius * Math.PI, 0);
                sec_circ_offset = Math.Round(2 * sec_radius * Math.PI, 0);
                min_circumference = Math.Round(2 * min_radius * Math.PI, 0);
                min_circ_offset = Math.Round(2 * min_radius * Math.PI, 0);
                hr_circumference = Math.Round(2 * hr_radius * Math.PI, 0);
                hr_circ_offset = Math.Round(2 * hr_radius * Math.PI, 0);
                sec_circ_tick = sec_circumference / 60;
                min_circ_tick = min_circumference / 60;
                hr_circ_tick = hr_circumference / 12;
                TokenSource = new CancellationTokenSource();
                ClockTask = RunClock(TokenSource.Token); //TokenSource.Token
                this.ThresholdReached += c_ThresholdReached;
                electronicDisplayer = BuildSVG_displayer((int)Math.Floor((double)SVG.width / 8), (int)(0.4 * (double)SVG.height), 0 + ":" + 0 + ":" + 0);

            }
        }

        private async Task RunClock(CancellationToken cancellationToken) //CancellationToken cancellationToken
        {
            // await Task.Delay(40);
            while (!cancellationToken.IsCancellationRequested) //!cancellationToken.IsCancellationRequested
            {
                DateTime Data = DateTime.Now;
                //  Console.WriteLine($"data : {Data}");
                UpdateSvg(Data);
                StateHasChanged();
                await Task.Delay(1000);
            }
        }
        private void UpdateSvg(DateTime date)
        {
            var hr = date.Hour;
            var min = date.Minute;
            // Console.WriteLine($"godzina: {hr}, {min}");
            var sec = date.Second;
            var secangle = sec * 6;
            var minangle = min * 6;
            var hourangle = hr * 30 + 5 * Math.Floor((double)min / 10);
            // Console.WriteLine($"tick: {hr%12}, {hr_circ_tick}");
            transform_second = "rotate(" + secangle + ",50,50)";
            transform_minute = "rotate(" + minangle + ",50,50)";
            transform_hour = "rotate(" + hourangle + ",50,50)";
            sec_circ_offset = sec_circumference - Math.Round(sec * sec_circ_tick, 0);
            min_circ_offset = min_circumference - Math.Round(min * min_circ_tick, 0);
            hr_circ_offset = hr_circumference - Math.Round(hr % 12 * hr_circ_tick, 0) - Math.Floor(hr_circ_tick * ((double)min / 60));
            var sec_radiusMargin = 20;
            options opt_sec = new options
            {
                radius = SVG.promien - sec_radiusMargin,
                klasa = "cien_seconds",
                value = Math.Round((double)secangle / 360, 2) * 100,
                transform = $"rotate(0 {SVG.width / 2 } {SVG.width / 2}) translate({sec_radiusMargin} {sec_radiusMargin})" // translate({SVG.width / 2 - SVG.promien + 8 + 4} {SVG.width / 2 - SVG.promien + 8 + 4} )
            };
            secondhandShadow = BuildSVG_secondShadow(opt_sec);
            if (sec == 0 || running == false)
            {
                running = true;
                minutehand = BuildSVG_minutehand(transform_minute);
                var min_radiusMargin = 25;
                options opt_min = new options
                {
                    radius = SVG.promien - min_radiusMargin,
                    klasa = "cien_minutes",
                    value = Math.Round((double)minangle / 360, 2) * 100,
                    transform = $"rotate(0 {SVG.width / 2 } {SVG.width / 2}) translate({min_radiusMargin} {min_radiusMargin})" // translate({SVG.width / 2 - SVG.promien + 8 + 4} {SVG.width / 2 - SVG.promien + 8 + 4} )
                };
                minutehandShadow = BuildSVG_minuteShadow(opt_min);
                var hr_radiusMargin = 30;
                options opt_hr = new options
                {
                    radius = SVG.promien - hr_radiusMargin,
                    klasa = "cien_hours",
                    value = Math.Round((30 * (hr % 12) + (double)min / 2) / 360, 2) * 100,
                    transform = $"rotate(0 {SVG.width / 2 } {SVG.width / 2}) translate({hr_radiusMargin} {hr_radiusMargin})" // translate({SVG.width / 2 - SVG.promien + 8 + 4} {SVG.width / 2 - SVG.promien + 8 + 4} )
                };
                // Console.WriteLine($"options: { opt.value}, {opt.transform}, {opt.radius}");
                hourhandShadow = BuildSVG_hourShadow(opt_hr);

                hourhand = BuildSVG_hourhand(transform_hour);
            }
            string Thours = (hr < 10 ? "0" : "") + hr.ToString();
            string Tminutes = (min < 10 ? "0" : "") + min.ToString();
            string Tseconds = (sec < 10 ? "0" : "") + sec.ToString();
            electronicDisplayer = BuildSVG_displayer((int)Math.Floor((double)SVG.width / 6), (int)(0.35 * (double)SVG.height), Thours + ":" + Tminutes + ":" + Tseconds);

        }
        private void c_ThresholdReached(object sender, ThresholdReachedEventArgs e)
        {
            Console.WriteLine("The threshold of {0} was reached at {1} by {2}.", e.Threshold, e.TimeReached, e.Name);
            electronicDisplayer = BuildSVG_displayer((int)Math.Floor((double)SVG.width / ((e.Name =="secondhand") ? 8 : 6)), (int)(0.35 * (double)SVG.height),(e.Name =="secondhand") ? "WWWOOOWWW" : "Boom!!!");
            Thresholdcounter = 0;

        }
        protected void IncrementBuilder(MouseEventArgs arg, metrics def)
        {
            Thresholdcounter += 1;
            Console.WriteLine($"invoked threshold {def.Name}, {def.klasa}, {arg.ClientX}, {arg.ClientY}");
            ThresholdReachedEventArgs args = new ThresholdReachedEventArgs();
            args.Threshold = Thresholdcounter;
            args.TimeReached = DateTime.Now;
            args.Name = def.Name;
            if (ThresholdcounterTotal <= Thresholdcounter)
            {
                OnThresholdReached(args);
            }

        }

        protected virtual void OnThresholdReached(ThresholdReachedEventArgs e)
        {
            EventHandler<ThresholdReachedEventArgs> handler = ThresholdReached;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private RenderFragment BuildSVG_hourhand(string transform = "") => builder =>
         {
             // <line id="hourhand" x1="50" y1="50" x2="50" y2="24" />
             var m = new metrics
             {
                 Name = "hourhand",
                 klasa = "n/a",
                 number = 202
             };
             builder.OpenElement(0, "line");
             builder.AddAttribute(1, "id", "hourhand");
             builder.AddAttribute(2, "x1", "50");
             builder.AddAttribute(3, "y1", "50");
             builder.AddAttribute(4, "x2", "50");
             builder.AddAttribute(5, "y2", "24");
             builder.AddAttribute(6, "onclick", Microsoft.AspNetCore.Components.EventCallback.Factory.Create<MouseEventArgs>(this, (MouseEventArgs e) => IncrementBuilder(e, m)));

             // builder.AddAttribute(6, "stroke", hr_color);
             builder.AddAttribute(7, "transform", transform);
             builder.CloseElement();

         };

        private RenderFragment BuildSVG_displayer(int x, int y, string text = "") => builder =>
         {
             // <line id="hourhand" x1="50" y1="50" x2="50" y2="24" />
             builder.OpenElement(0, "text");
             builder.AddAttribute(1, "id", "displayer");
             builder.AddAttribute(2, "x", x);
             builder.AddAttribute(3, "y", y);
             builder.AddAttribute(4, "class", "zegar_text_digital");
             builder.AddContent(5, text);
             builder.CloseElement();

         };

        private RenderFragment BuildSVG_minutehand(string transform = "") => builder =>
          {
              // <line id="minutehand" x1="50" y1="50" x2="50" y2="20" />
              var m = new metrics
              {
                  Name = "minutehand",
                  klasa = "n/a",
                  number = 203
              };
              builder.OpenElement(0, "line");
              builder.AddAttribute(1, "id", "minutehand");
              builder.AddAttribute(2, "x1", "50");
              builder.AddAttribute(3, "y1", "50");
              builder.AddAttribute(4, "x2", "50");
              builder.AddAttribute(5, "y2", "20");
              builder.AddAttribute(6, "onclick", Microsoft.AspNetCore.Components.EventCallback.Factory.Create<MouseEventArgs>(this, (MouseEventArgs e) => IncrementBuilder(e, m)));
              // builder.AddAttribute(6, "stroke", min_color);
              builder.AddAttribute(7, "transform", transform);
              builder.CloseElement();

          };

        private RenderFragment BuildSVG_minuteShadow(options opt)
        => builder =>
        {
            //radius,value,klasa,transform
            // Console.WriteLine($"opt.value  {opt.value}");
            var m = new metrics
            {
                Name = "minutehandShadow",
                klasa = opt.klasa,
                number = 20
            };
            if (opt.value > 0)
            {
                var x = Math.Cos((2 * Math.PI) / (100 / opt.value));
                var y = Math.Sin((2 * Math.PI) / (100 / opt.value));
                //should the arc go the long way round?
                var longArc = (opt.value <= 50) ? 0 : 1;
                //d is a string that describes the path of the slice.
                var d = "M" + opt.radius + "," + opt.radius + " L" + opt.radius + "," + 0 + " A" + opt.radius + "," + opt.radius + " 0 " + longArc + ",1 " + (int)Math.Round(opt.radius + (y * opt.radius), 0) + "," + (int)Math.Round(opt.radius - (x * opt.radius), 0) + " z";
                // Console.WriteLine($"d: {d}");
                builder.OpenElement(0, "path");
                builder.AddAttribute(1, "id", "minutehandShadow");
                builder.AddAttribute(2, "d", d);
                builder.AddAttribute(3, "class", opt.klasa);
                builder.AddAttribute(4, "transform", opt.transform);
                builder.AddAttribute(5, "onclick", Microsoft.AspNetCore.Components.EventCallback.Factory.Create<MouseEventArgs>(this, (MouseEventArgs e) => IncrementBuilder(e, m)));
                builder.CloseElement();
            }
        };
        private RenderFragment BuildSVG_secondShadow(options opt)
        => builder =>
        {
            //radius,value,klasa,transform
            // Console.WriteLine($"opt.value  {opt.value}");
            if (opt.value > 0)
            {
                var x = Math.Cos((2 * Math.PI) / (100 / opt.value));
                var y = Math.Sin((2 * Math.PI) / (100 / opt.value));
                //should the arc go the long way round?
                var longArc = (opt.value <= 50) ? 0 : 1;
                //d is a string that describes the path of the slice.
                var d = "M" + opt.radius + "," + opt.radius + " L" + opt.radius + "," + 0 + " A" + opt.radius + "," + opt.radius + " 0 " + longArc + ",1 " + Convert.ToString(Math.Round(opt.radius + (y * opt.radius), 2)).Replace(",", ".") + "," + Convert.ToString(Math.Round(opt.radius - (x * opt.radius), 2)).Replace(",", ".") + " z";
                //   Console.WriteLine($"d: {d}");
                var m = new metrics
                {
                    Name = "secondhandShadow",
                    klasa = opt.klasa,
                    number = 2
                };
                builder.OpenElement(0, "path");
                builder.AddAttribute(1, "id", "secondhandShadow");
                builder.AddAttribute(2, "d", d);
                builder.AddAttribute(3, "class", opt.klasa);
                builder.AddAttribute(4, "onclick", Microsoft.AspNetCore.Components.EventCallback.Factory.Create<MouseEventArgs>(this, (MouseEventArgs e) => IncrementBuilder(e, m)));
                builder.AddAttribute(5, "transform", opt.transform);
                builder.CloseElement();

            }
        };

        private RenderFragment BuildSVG_hourShadow(options opt)
        => builder =>
        {
            //radius,value,klasa,transform
            // Console.WriteLine($"opt.value  {opt.value}");
            if (opt.value > 0)
            {
                var x = Math.Cos((2 * Math.PI) / (100 / opt.value));
                var y = Math.Sin((2 * Math.PI) / (100 / opt.value));
                //should the arc go the long way round?
                var longArc = (opt.value <= 50) ? 0 : 1;
                //d is a string that describes the path of the slice.
                var d = "M" + opt.radius + "," + opt.radius + " L" + opt.radius + "," + 0 + " A" + opt.radius + "," + opt.radius + " 0 " + longArc + ",1 " + (int)Math.Round(opt.radius + (y * opt.radius), 0) + "," + (int)Math.Round(opt.radius - (x * opt.radius), 0) + " z";
                //  Console.WriteLine($"d: {d}");
                var m = new metrics
                {
                    Name = "hourhandShadow",
                    klasa = opt.klasa,
                    number = 1
                };
                builder.OpenElement(0, "path");
                builder.AddAttribute(1, "id", "hourhandShadow");
                builder.AddAttribute(2, "d", d);
                builder.AddAttribute(3, "class", opt.klasa);
                builder.AddAttribute(4, "transform", opt.transform);
                builder.AddAttribute(5, "onclick", Microsoft.AspNetCore.Components.EventCallback.Factory.Create<MouseEventArgs>(this, (MouseEventArgs e) => IncrementBuilder(e, m)));
                builder.CloseElement();
            }
        };

        public void Dispose()
        {

            ClockTask = null;
            Console.WriteLine("Object has been destroyed");

        }

    }
}