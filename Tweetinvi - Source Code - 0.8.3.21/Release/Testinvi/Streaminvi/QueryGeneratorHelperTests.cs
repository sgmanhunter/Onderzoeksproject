using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Streaminvi.Helpers;

namespace Testinvi.Streaminvi
{
    [TestClass]
    public class QueryGeneratorHelperTests
    {
        #region GenerateFilterTrackRequest
        [TestMethod]
        public void GenerateFilterTrackRequest_EmptyTrack_CorrectParameterString()
        {
            // Act
            string trackParameter = QueryGeneratorHelper.GenerateFilterTrackRequest(new List<string>());

            // Assert
            Assert.AreEqual(trackParameter, "");
        }

        [TestMethod]
        public void GenerateFilterTrackRequest_SingleTrack_CorrectParameterString()
        {
            // Arrange
            const string track1 = "Track1 is good";
            List<string> tracks = new List<string> { track1 };

            // Act
            string trackParameter = QueryGeneratorHelper.GenerateFilterTrackRequest(tracks);

            // Assert
            Assert.AreEqual(trackParameter, String.Format("track={0}", Uri.EscapeDataString(track1)));
        }

        [TestMethod]
        public void GenerateFilterTrackRequest_2Tracks_CorrectParameterString()
        {
            // Arrange
            const string track1 = "Track1 is good";
            const string track2 = "Track2 is too";

            List<string> tracks = new List<string> { track1, track2 };

            // Act
            string trackParameter = QueryGeneratorHelper.GenerateFilterTrackRequest(tracks);

            // Assert
            Assert.AreEqual(trackParameter, String.Format("track={0}%2C{1}",
                Uri.EscapeDataString(track1),
                Uri.EscapeDataString(track2)));
        } 
        #endregion

        // TODO : Follow + Location
    }
}
