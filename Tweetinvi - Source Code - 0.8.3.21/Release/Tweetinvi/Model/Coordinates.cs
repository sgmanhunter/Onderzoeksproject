using TweetinCore.Interfaces;

namespace Tweetinvi.Model
{
    /// <summary>
    /// Coordinates of a geographical location
    /// </summary>
    public class Coordinates : ICoordinates
    {
        #region Private Attributes

        private double _lattitude;
        private double _longitude; 
        
        #endregion

        #region Public Attributes

        public double Lattitude
        {
            get { return _lattitude; }
            set { _lattitude = value; }
        }

        public double Longitude
        {
            get { return _longitude; }
            set { _longitude = value; }
        }

        #endregion

        /// <summary>
        /// Create coordinates with its latitude and longitude
        /// </summary>
        public Coordinates(double longitude, double latitude)
        {
            _longitude = longitude; 
            _lattitude = latitude;
        }
    }
}
