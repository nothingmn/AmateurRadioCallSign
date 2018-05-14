using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AmateurRadioCallSign
{
    public class DataManager
    {
        public AmateurRadioCallSignExport CanadianCallSigns { get; set; }

        public Dictionary<string, AmateurRadioCallSign> CanadianClubs { get; private set; } = new Dictionary<string, AmateurRadioCallSign>();

        public void Initialize()
        {
            var asm = typeof(DataManager).Assembly;

            var resource1 = (from m in asm.GetManifestResourceNames()
                             where m.Contains("callsigns.canada")
                             select m)?.FirstOrDefault();

            using (var stm = asm.GetManifestResourceStream(resource1))
            {
                byte[] buffer = new byte[stm.Length];
                stm.Read(buffer, 0, buffer.Length);
                CanadianCallSigns = JsonConvert.DeserializeObject<AmateurRadioCallSignExport>(System.Text.Encoding.UTF8.GetString(buffer));
            }

            foreach (var entry in CanadianCallSigns.CallSigns)
            {
                var clubName = entry.Value.ClubName.ToUpperInvariant();
                if (!string.IsNullOrEmpty(clubName) && !CanadianClubs.ContainsKey(clubName))
                {
                    var club = new AmateurRadioCallSign()
                    {
                        ClubName = clubName,
                        ClubPostalCode = entry.Value.ClubPostalCode,
                        ClubAddress = entry.Value.ClubAddress,
                        ClubCity = entry.Value.ClubCity,
                        ClubProvince = entry.Value.ClubProvince
                    };
                    CanadianClubs.Add(clubName, club);
                }
            }
        }

        public int? Count
        {
            get { return CanadianCallSigns?.CallSigns?.Count; }
        }

        public IEnumerable<AmateurRadioCallSign> FindAll(Func<AmateurRadioCallSign, bool> searchPredicate)
        {
            return from v in CanadianCallSigns?.CallSigns.Values.Where(searchPredicate) select v;
        }

        public AmateurRadioCallSign GetByCallSign(string callSign)
        {
            if (string.IsNullOrEmpty(callSign)) return null;

            if (CanadianCallSigns.CallSigns.ContainsKey(callSign.ToUpperInvariant()))
            {
                return CanadianCallSigns.CallSigns[callSign.ToUpperInvariant()];
            }

            //slower, but an acceptable fallback when the dictionary match fails
            return (from c in CanadianCallSigns.CallSigns.Values
                    where c.CallSign.Replace(" ", "").Equals(callSign.Replace(" ", ""), StringComparison.InvariantCultureIgnoreCase)
                    select c)?.FirstOrDefault();
        }
    }
}