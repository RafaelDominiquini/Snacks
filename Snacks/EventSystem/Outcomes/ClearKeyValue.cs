﻿/**
The MIT License (MIT)
Copyright (c) 2014-2019 by Michael Billard
 

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 * */
namespace Snacks
{
    /// <summary>
    /// This outcome removes the desired key-value from the affected kerbals
    /// Example definition:
    /// OUTCOME 
    /// {
    ///     name  = ClearKeyValues
    ///     conditionSummary = Sick
    /// }
    /// </summary>  
    public class ClearKeyValue : BaseOutcome
    {
        #region Housekeeping
        /// <summary>
        /// Name of the key-value
        /// </summary>
        public string keyValueName = string.Empty;
        #endregion

        #region Constants
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Snacks.ClearKeyValue"/> class.
        /// </summary>
        /// <param name="node">A ConfigNode containing initialization parameters. Parameters in the
        /// <see cref="T:Snacks.BaseOutcome"/> class also apply.</param>
        public ClearKeyValue(ConfigNode node): base(node)
        {
            if (node.HasValue(SetKeyValue.NameKeyValue))
                keyValueName = node.GetValue(SetKeyValue.NameKeyValue);
        }
        #endregion

        #region Overrides
        public override void ApplyOutcome(Vessel vessel, SnacksProcessorResult result)
        {
            ProtoCrewMember[] astronauts;
            AstronautData astronautData;

            //Get affected astronauts
            if (result.afftectedAstronauts != null)
                astronauts = result.afftectedAstronauts.ToArray();
            else if (vessel.loaded)
                astronauts = vessel.GetVesselCrew().ToArray();
            else
                astronauts = vessel.protoVessel.GetVesselCrew().ToArray();

            //Select random crew if needed
            if (selectRandomCrew)
            {
                int randomIndex = UnityEngine.Random.Range(0, astronauts.Length - 1);
                astronauts = new ProtoCrewMember[] { astronauts[randomIndex] };
            }

            //Now apply the condition
            for (int index = 0; index < astronauts.Length; index++)
            {
                astronautData = SnacksScenario.Instance.GetAstronautData(astronauts[index]);
                if (astronautData == null)
                    continue;

                if (astronautData.keyValuePairs.ContainsKey(keyValueName))
                    astronautData.keyValuePairs.Remove(keyValueName);

                SnacksScenario.Instance.SetAstronautData(astronautData);
            }

            //Call the base class
            base.ApplyOutcome(vessel, result);
        }
        #endregion
    }
}
