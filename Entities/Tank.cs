using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SCADA_Project.Entities
{
    public class Tank
    {
        public int TankId { get; set; }
        public double AlarmLevel { get; set; }
        public double ThresholdLevel { get; set; }
        public double HighLevel { get; set; }
        public double Level { get; set; }
        public double Capacity { get; set; }
        public bool Pumping { get; set; }
        public bool Filling { get; set; }
        public bool Busy { get; set; }
        public double PumpingSpeed { get; set; }
        public double FillingSpeed { get; set; }

        private CheckBox checkAlarm;
        private CheckBox checkAlarmbackup;
        private CheckBox checkThreshold;
        private CheckBox checkHighlevel;
        private System.Windows.Forms.Timer timerTank;
        private PictureBox picWaste;
        private PictureBox picPump;
        private PictureBox picTank;
        private Label labelPumping;
        private Label labelOver;
        private Label labelNotEmpty;
        private Label labelTank;
        private Button buttonPump;
        private TextBox textEcho;


        public Tank(int tankId, double alarmLevel, double thresholdLevel, double level, double highlevel, double capacity,
            bool pumping, bool filling,
            double pumpingSpeed, double fillingSpeed,
            CheckBox checkAlarm, CheckBox checkAlarmbackup, CheckBox checkThreshold, CheckBox checkHighlevel,
            System.Windows.Forms.Timer timerTank, 
            PictureBox picWaste, PictureBox picPump, PictureBox picTank,
            Label labelPumping, Label labelOver, Label labelNotEmpty,
            Label labeltank, Button buttonPump, TextBox textEcho)
        {
            TankId = tankId;
            AlarmLevel = alarmLevel;
            ThresholdLevel = thresholdLevel;
            HighLevel = highlevel;
            Level = level;
            Capacity = capacity;
            Pumping = pumping;
            Filling = filling;
            Busy = false;
            PumpingSpeed = pumpingSpeed;
            FillingSpeed = fillingSpeed;
            this.checkAlarm = checkAlarm;
            this.checkAlarmbackup = checkAlarmbackup;
            this.checkThreshold = checkThreshold;
            this.checkHighlevel = checkHighlevel;
            this.timerTank = timerTank;
            this.picWaste = picWaste;
            this.picTank = picTank;
            this.picPump = picPump;
            this.labelPumping = labelPumping;
            this.labelOver = labelOver;
            this.labelNotEmpty = labelNotEmpty;
            this.labelTank = labeltank;
            this.buttonPump = buttonPump;
            this.textEcho = textEcho;
        }

        public void echo(String msg)
        {
            textEcho.AppendText(Environment.NewLine + System.DateTime.Now + "  :"
                + "   Waste Level: " +Level.ToString("F2")
                + " Liters    -    "+msg);
        }

        public void PumpingOn()
        {
            Pumping = true;
            echo("Pumping Enabled");
            picPump.Image = Properties.Resources.pumpR;
            buttonPump.BackgroundImage = Properties.Resources.ButtonON;
            labelPumping.ForeColor = Color.Yellow;
            picPump.Update();
            buttonPump.Update();
        }

        public void PumpingOff()
        {
            Pumping = false; picPump.Image = null;
            echo("Pumping Disabled");
            buttonPump.BackgroundImage = Properties.Resources.ButtonOFF;
            labelPumping.ForeColor = Color.DimGray;
            picPump.Update();
            buttonPump.Update();
        }

        public void updateLevel()
            {
               if(Level >= ThresholdLevel)       //защелка
                    {
                        checkThreshold.Checked=true;
                        labelNotEmpty.ForeColor = Color.GreenYellow;
                    }
               if (Level >= HighLevel) checkHighlevel.Checked = true; //верхний уровень
               if (Pumping)                     //отбор
                   {
                   if (Level >= ThresholdLevel)
                         Level -= PumpingSpeed;
                   else                         //ниже защелки
                        {
                            labelNotEmpty.ForeColor = Color.DimGray;
                            labelOver.ForeColor = Color.DimGray;
                            checkAlarm.Checked = false;
                            checkAlarmbackup.Checked = false;
                            checkHighlevel.Checked = false;
                            checkThreshold.Checked = false;
                            if(Pumping) PumpingOff();
                        }
                   }
               else
                   if (checkHighlevel.Checked) PumpingOn();
               if (Filling)                                     //кран
                   {
                   if (Level < AlarmLevel)
                       Level += FillingSpeed;
                   }
               if (checkAlarm.Checked || checkAlarmbackup.Checked || Level >= AlarmLevel) //авария 
                   {
                      labelOver.ForeColor = Color.Red;
                      if(Filling) Filling = false;
                      checkAlarm.Checked = true;
                      checkAlarmbackup.Checked = true;
                      if(!Pumping) PumpingOn();
                   }

                labelTank.Text = Level<100?Level.ToString("F2")+"L": Level.ToString("F1") + "L";  
                picWaste.Height = (int)(Level*((double)(0.83*picTank.Height)/Capacity));
                picWaste.Top = picTank.Top+picTank.Height-picWaste.Height-1;
                picWaste.Update();
            }
    }
}
