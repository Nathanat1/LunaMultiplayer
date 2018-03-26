﻿using LunaCommon.Xml;
using Server.Utilities;
using System.Collections.Concurrent;
using System.Globalization;
using System.Threading.Tasks;
using System.Xml;

namespace Server.System
{
    public class ScenarioDataUpdater
    {
        #region Semaphore

        /// <summary>
        /// To not overwrite our own data we use a lock
        /// </summary>
        private static readonly ConcurrentDictionary<string, object> Semaphore = new ConcurrentDictionary<string, object>();

        #endregion

        /// <summary>
        /// Raw updates a scenario in the dictionary
        /// </summary>
        public static void RawConfigNodeInsertOrUpdate(string scenarioModule, string scenarioDataInConfigNodeFormat)
        {
            Task.Run(() =>
            {
                lock (Semaphore.GetOrAdd(scenarioModule, new object()))
                {
                    var scenarioAsXml = ConfigNodeXmlParser.ConvertToXml(scenarioDataInConfigNodeFormat);
                    ScenarioStoreSystem.CurrentScenariosInXmlFormat.AddOrUpdate(scenarioModule, scenarioAsXml, (key, existingVal) => scenarioAsXml);
                }
            });
        }

        #region Funds

        /// <summary>
        /// We received a funds message so update the scenario file accordingly
        /// </summary>
        public static void WriteFundsDataToFile(double funds)
        {
            Task.Run(() =>
            {
                lock (Semaphore.GetOrAdd("Funding", new object()))
                {
                    if (!ScenarioStoreSystem.CurrentScenariosInXmlFormat.TryGetValue("Funding", out var xmlData)) return;

                    var updatedText = UpdateScenarioWithFundsData(xmlData, funds);
                    ScenarioStoreSystem.CurrentScenariosInXmlFormat.TryUpdate("Funding", updatedText, xmlData);
                }
            });
        }

        /// <summary>
        /// Patches the scenario file with funds data
        /// </summary>
        /// <returns></returns>
        private static string UpdateScenarioWithFundsData(string scenarioData, double funds)
        {
            var document = new XmlDocument();
            document.LoadXml(scenarioData);

            var node = document.SelectSingleNode($"/{ConfigNodeXmlParser.StartElement}/{ConfigNodeXmlParser.ValueNode}[@name='funds']");
            if (node != null) node.InnerText = funds.ToString(CultureInfo.InvariantCulture);

            return document.ToIndentedString();
        }

        #endregion

        #region Science

        /// <summary>
        /// We received a science message so update the scenario file accordingly
        /// </summary>
        public static void WriteScienceDataToFile(float sciencePoints)
        {
            Task.Run(() =>
            {
                lock (Semaphore.GetOrAdd("ResearchAndDevelopment", new object()))
                {
                    if (!ScenarioStoreSystem.CurrentScenariosInXmlFormat.TryGetValue("ResearchAndDevelopment", out var xmlData)) return;

                    var updatedText = UpdateScenarioWithScienceData(xmlData, sciencePoints);
                    ScenarioStoreSystem.CurrentScenariosInXmlFormat.TryUpdate("ResearchAndDevelopment", updatedText, xmlData);
                }
            });
        }

        /// <summary>
        /// Patches the scenario file with science data
        /// </summary>
        /// <returns></returns>
        private static string UpdateScenarioWithScienceData(string scenarioData, float sciencePoints)
        {
            var document = new XmlDocument();
            document.LoadXml(scenarioData);

            var node = document.SelectSingleNode($"/{ConfigNodeXmlParser.StartElement}/{ConfigNodeXmlParser.ValueNode}[@name='sci']");
            if (node != null) node.InnerText = sciencePoints.ToString(CultureInfo.InvariantCulture);

            return document.ToIndentedString();
        }

        #endregion

        #region Reputation

        /// <summary>
        /// We received a reputation message so update the scenario file accordingly
        /// </summary>
        public static void WriteReputationDataToFile(float reputationPoints)
        {
            Task.Run(() =>
            {
                lock (Semaphore.GetOrAdd("Reputation", new object()))
                {
                    if (!ScenarioStoreSystem.CurrentScenariosInXmlFormat.TryGetValue("Reputation", out var xmlData)) return;

                    var updatedText = UpdateScenarioWithReputationData(xmlData, reputationPoints);
                    ScenarioStoreSystem.CurrentScenariosInXmlFormat.TryUpdate("Reputation", updatedText, xmlData);
                }
            });
        }

        /// <summary>
        /// Patches the scenario file with reputation data
        /// </summary>
        /// <returns></returns>
        private static string UpdateScenarioWithReputationData(string scenarioData, float reputationPoints)
        {
            var document = new XmlDocument();
            document.LoadXml(scenarioData);

            var node = document.SelectSingleNode($"/{ConfigNodeXmlParser.StartElement}/{ConfigNodeXmlParser.ValueNode}[@name='rep']");
            if (node != null) node.InnerText = reputationPoints.ToString(CultureInfo.InvariantCulture);

            return document.ToIndentedString();
        }

        #endregion
    }
}
