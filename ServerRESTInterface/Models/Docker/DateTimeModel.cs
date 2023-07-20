namespace ServerRESTInterface.Models.Docker
{
    public class DateTimeModel
    {
        readonly DateTime _dateTime;
        public double UnixEpoch => _dateTime.Subtract(DateTime.UnixEpoch.ToUniversalTime()).TotalSeconds;

        public int day => _dateTime.Day;
        public int month => _dateTime.Month;
        public int year => _dateTime.Year;

        public int hour => _dateTime.Hour;
        public int minute => _dateTime.Minute;
        public int second => _dateTime.Second;

        public string TimeZone => _dateTime.Kind.ToString().ToUpper();


        public DateTimeModel(DateTime dateTime)
        {
            this._dateTime = dateTime.ToUniversalTime();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTimeString">Format: yyyy'-'MM'-'dd'T'hh':'mm':'ss'Z'</param>
        public DateTimeModel(string dateTimeString)
        {
            string[] splitDateTime = dateTimeString.Split('T');
            string[] date = splitDateTime[0].Split('-');

            splitDateTime[1] = splitDateTime[1].Replace("Z", String.Empty);
            string[] time = splitDateTime[1].Split(':');

            int year = Convert.ToInt32(date[0]);
            int month = Convert.ToInt32(date[1]);
            int day = Convert.ToInt32(date[2]);
            int hour = Convert.ToInt32(time[0]);
            int minute = Convert.ToInt32(time[1]);
            int second = Convert.ToInt32(time[2]);

            DateTime dateTime = new DateTime(year, month, day, hour, minute, second);
            _dateTime = dateTime.ToUniversalTime();
        }

        public DateTimeModel()
        {
            _dateTime = DateTime.Now.ToUniversalTime();
        }
    }
}
