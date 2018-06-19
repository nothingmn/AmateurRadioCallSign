using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using AmateurRadioCallSign;

namespace AmateurRadioCallSignConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var dataManager = new DataManager();
            dataManager.Initialize();
            while (true)
            {
                Console.WriteLine("Enter a Callsign or search input:");
                var sign = Console.ReadLine();
                if (sign == "quit" || sign == "exit") break;

                if (sign.StartsWith("clubs", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (sign.Equals("clubs", StringComparison.InvariantCultureIgnoreCase) || !sign.Contains(" "))
                    {
                        foreach (var club in dataManager.CanadianClubs)
                        {
                            DumpClub(club.Value);
                        }
                    }
                    else
                    {
                        var rightSide = sign.Substring(sign.IndexOf(" ", StringComparison.InvariantCultureIgnoreCase))?.Trim();
                        if (!string.IsNullOrEmpty(rightSide))
                        {
                            foreach (var club in from c in dataManager.CanadianClubs where c.Value.ClubCity.Equals(rightSide, StringComparison.InvariantCultureIgnoreCase) || c.Value.ClubPostalCode.Equals(rightSide, StringComparison.InvariantCultureIgnoreCase) select c)
                            {
                                DumpClub(club.Value);
                            }
                        }
                    }

                    continue;
                }

                Console.WriteLine(new string('-', 10));
                var data = dataManager.GetByCallSign(sign);
                if (data == null)
                {
                    //slower than a lookup by callsign, but useful for scanning over the data
                    var many = dataManager.FindAll(callSign =>
                        callSign.CallSign.Equals(sign, StringComparison.InvariantCultureIgnoreCase) ||
                        (!string.IsNullOrEmpty(callSign.Address) && callSign.Address.Equals(sign, StringComparison.InvariantCultureIgnoreCase)) ||
                        (!string.IsNullOrEmpty(callSign.City) && callSign.City.Equals(sign, StringComparison.InvariantCultureIgnoreCase)) ||
                        (!string.IsNullOrEmpty(callSign.ClubName) && callSign.ClubName.Equals(sign, StringComparison.InvariantCultureIgnoreCase)) ||
                        (!string.IsNullOrEmpty(callSign.ClubAddress) && callSign.ClubAddress.Equals(sign, StringComparison.InvariantCultureIgnoreCase)) ||
                        (!string.IsNullOrEmpty(callSign.ClubCity) && callSign.ClubCity.Equals(sign, StringComparison.InvariantCultureIgnoreCase)) ||
                        (!string.IsNullOrEmpty(callSign.ClubPostalCode) && callSign.ClubPostalCode.Equals(sign, StringComparison.InvariantCultureIgnoreCase)) ||
                        (!string.IsNullOrEmpty(callSign.ClubProvince) && callSign.ClubProvince.Equals(sign, StringComparison.InvariantCultureIgnoreCase)) ||
                        (!string.IsNullOrEmpty(callSign.SecondClubName) && callSign.SecondClubName.Equals(sign, StringComparison.InvariantCultureIgnoreCase)) ||
                        (!string.IsNullOrEmpty(callSign.GivenNames) && callSign.GivenNames.Equals(sign, StringComparison.InvariantCultureIgnoreCase)) ||
                        (!string.IsNullOrEmpty(callSign.PostalCode) && callSign.PostalCode.Equals(sign, StringComparison.InvariantCultureIgnoreCase)) ||
                        (!string.IsNullOrEmpty(callSign.Province) && callSign.Province.Equals(sign, StringComparison.InvariantCultureIgnoreCase)) ||
                        (!string.IsNullOrEmpty(callSign.SurName) && callSign.SurName.Equals(sign, StringComparison.InvariantCultureIgnoreCase))
                    );
                    Console.WriteLine(new string('=', 10));
                    Console.WriteLine("Search Results");
                    Console.WriteLine(new string('=', 10));

                    foreach (var result in many)
                    {
                        Console.WriteLine(new string('-', 10));
                        OutputSingle(result);
                    }
                }

                if (data != null)
                {
                    OutputSingle(data);
                }
                else
                {
                    Console.WriteLine("Could not find any data for that call sign.");
                }
                Console.WriteLine(new string('-', 10));
            }
        }

        private static void DumpClub(AmateurRadioCallSign.AmateurRadioCallSign club)
        {
            Console.WriteLine($"--------Club--------");
            Console.WriteLine($"       Name:{club.ClubName}");
            Console.WriteLine($"    Address:{club.ClubAddress}");
            Console.WriteLine($"       City:{club.ClubCity}");
            Console.WriteLine($"Postal Code:{club.ClubPostalCode}");
            Console.WriteLine($"   Province:{club.ClubProvince}");
        }

        private static void OutputSingle(AmateurRadioCallSign.AmateurRadioCallSign sign)
        {
            if (sign == null) return;
            var type = sign.GetType();
            var properties = from p in type.GetProperties() where p.CanRead select p;
            foreach (var p in properties)
            {
                var value = p.GetValue(sign)?.ToString();
                if (!string.IsNullOrEmpty(value)) Console.WriteLine($"{p.Name} = {value}");
            }
        }
    }
}