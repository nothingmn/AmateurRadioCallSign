using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
        public AmateurRadioCallSignExport USCallSigns { get; private set; }
        public AmateurRadioCallSignExport CANCallSigns { get; private set; }
        public IDictionary<string, AmateurRadioCallSign> Combined { get; private set; }

        public Dictionary<string, AmateurRadioCallSign> CanadianClubs { get; private set; } = new Dictionary<string, AmateurRadioCallSign>();

        public void Initialize()
        {
            var asm = typeof(DataManager).Assembly;

            var zipResource = (from m in asm.GetManifestResourceNames()
                               where m.Contains("callsigns")
                               select m)?.FirstOrDefault();

            Dictionary<string, byte[]> files = null;
            using (var stm = asm.GetManifestResourceStream(zipResource))
            {
                //byte[] buffer = new byte[stm.Length];
                //stm.Read(buffer, 0, buffer.Length);
                //CallSigns = JsonConvert.DeserializeObject<AmateurRadioCallSignExport>(System.Text.Encoding.UTF8.GetString(buffer));
                //var
                files = ExtractZip(stm);
            }

            if (files != null)
            {
                var us = (from f in files where f.Key == "callsigns.usa.json" select f)?.FirstOrDefault();
                var can = (from f in files where f.Key == "callsigns.canada.json" select f)?.FirstOrDefault();

                USCallSigns = JsonConvert.DeserializeObject<AmateurRadioCallSignExport>(System.Text.Encoding.UTF8.GetString(us.Value.Value));
                CANCallSigns = JsonConvert.DeserializeObject<AmateurRadioCallSignExport>(System.Text.Encoding.UTF8.GetString(can.Value.Value));

                Combined = (new List<Dictionary<string, AmateurRadioCallSign>>()
                    {
                        USCallSigns.CallSigns,
                        CANCallSigns.CallSigns
                    }).SelectMany(dict => dict)
                    .ToLookup(pair => pair.Key, pair => pair.Value)
                    .ToDictionary(group => group.Key, group => group.First());

                foreach (var entry in CANCallSigns.CallSigns)
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
        }

        private Dictionary<string, byte[]> ExtractZip(System.IO.Stream zipBuffer)
        {
            var contents = new Dictionary<string, byte[]>();

            using (ZipArchive archive = new ZipArchive(zipBuffer, ZipArchiveMode.Read, false))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    using (var stm = entry.Open())
                    {
                        var fileBuffer = new byte[entry.Length];
                        stm.Read(fileBuffer, 0, fileBuffer.Length);
                        contents.Add(entry.FullName, fileBuffer);
                    }
                }
            }

            return contents;
        }

        public int? Count
        {
            get { return Combined.Count; }
        }

        public IEnumerable<AmateurRadioCallSign> FindAll(Func<AmateurRadioCallSign, bool> searchPredicate)
        {
            return from v in Combined.Values.Where(searchPredicate) select v;
        }

        public AmateurRadioCallSign GetByCallSign(string callSign)
        {
            if (string.IsNullOrEmpty(callSign)) return null;

            if (Combined.ContainsKey(callSign.ToUpperInvariant()))
            {
                return Combined[callSign.ToUpperInvariant()];
            }

            //slower, but an acceptable fallback when the dictionary match fails
            return (from c in Combined.Values
                    where c.CallSign.Replace(" ", "").Equals(callSign.Replace(" ", ""), StringComparison.InvariantCultureIgnoreCase)
                    select c)?.FirstOrDefault();
        }
    }
}