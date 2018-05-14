namespace AmateurRadioCallSign
{
    public class AmateurRadioCallSign
    {
        public string CallSign { get; set; }
        public string GivenNames { get; set; }
        public string SurName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public Qualifications Qualifications { get; set; }
        public string ClubName { get; set; }
        public string SecondClubName { get; set; }
        public string ClubAddress { get; set; }
        public string ClubCity { get; set; }
        public string ClubProvince { get; set; }
        public string ClubPostalCode { get; set; }

        public override string ToString()
        {
            return $"{CallSign} {GivenNames} {SurName} {Address} {City} {Province} {PostalCode} {Qualifications}";
        }
    }
}