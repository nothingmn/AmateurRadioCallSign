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
                            Console.WriteLine($"--------Club--------");
                            Console.WriteLine($"       Name:{club.Value.ClubName}");
                            Console.WriteLine($"    Address:{club.Value.ClubAddress}");
                            Console.WriteLine($"       City:{club.Value.ClubCity}");
                            Console.WriteLine($"Postal Code:{club.Value.ClubPostalCode}");
                            Console.WriteLine($"   Province:{club.Value.ClubProvince}");
                        }
                    }
                    else
                    {
                        var rightSide = sign.Substring(sign.IndexOf(" ", StringComparison.InvariantCultureIgnoreCase))?.Trim();
                        if (!string.IsNullOrEmpty(rightSide))
                        {
                            foreach (var club in from c in dataManager.CanadianClubs where c.Value.ClubCity.Equals(rightSide, StringComparison.InvariantCultureIgnoreCase) || c.Value.ClubPostalCode.Equals(rightSide, StringComparison.InvariantCultureIgnoreCase) select c)
                            {
                                Console.WriteLine($"--------Club--------");
                                Console.WriteLine($"       Name:{club.Value.ClubName}");
                                Console.WriteLine($"    Address:{club.Value.ClubAddress}");
                                Console.WriteLine($"       City:{club.Value.ClubCity}");
                                Console.WriteLine($"Postal Code:{club.Value.ClubPostalCode}");
                                Console.WriteLine($"   Province:{club.Value.ClubProvince}");
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
                        callSign.Address.Equals(sign, StringComparison.InvariantCultureIgnoreCase) ||
                        callSign.City.Equals(sign, StringComparison.InvariantCultureIgnoreCase) ||
                        callSign.ClubName.Equals(sign, StringComparison.InvariantCultureIgnoreCase) ||
                        callSign.ClubAddress.Equals(sign, StringComparison.InvariantCultureIgnoreCase) ||
                        callSign.ClubCity.Equals(sign, StringComparison.InvariantCultureIgnoreCase) ||
                        callSign.ClubPostalCode.Equals(sign, StringComparison.InvariantCultureIgnoreCase) ||
                        callSign.ClubProvince.Equals(sign, StringComparison.InvariantCultureIgnoreCase) ||
                        callSign.SecondClubName.Equals(sign, StringComparison.InvariantCultureIgnoreCase) ||
                        callSign.GivenNames.Equals(sign, StringComparison.InvariantCultureIgnoreCase) ||
                        callSign.PostalCode.Equals(sign, StringComparison.InvariantCultureIgnoreCase) ||
                        callSign.Province.Equals(sign, StringComparison.InvariantCultureIgnoreCase) ||
                        callSign.SurName.Equals(sign, StringComparison.InvariantCultureIgnoreCase)
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