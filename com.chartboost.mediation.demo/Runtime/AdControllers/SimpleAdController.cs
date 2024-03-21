using System.Collections.Generic;

namespace Chartboost.Mediation.Demo.AdControllers
{
    /// <summary>
    /// Simple Advertisement controller class, does not care for specifics.
    /// </summary>
    public abstract class SimpleAdController
    {
        protected readonly string PlacementIdentifier;

        protected SimpleAdController(string placementIdentifier)
        {
            PlacementIdentifier = placementIdentifier;
        }

        protected readonly Dictionary<string, string> DefaultKeywords = new Dictionary<string, string>()
        {
            { "i12_keyword1", "i12_value1" },
            { "i12_keyword2", "i12_value2" }
        };

        public abstract void Load();

        public abstract void Show();

        public abstract void Invalidate();
    }
}
