namespace Server.Customs.Encryption
{
    public class Configuration
    {
        public static bool Enabled = true;

        public static bool AllowUnencryptedClients = true;

        public static LoginKey[] LoginKeys = new LoginKey[]
		{
            new LoginKey("7.0.45 2D",       0x2644752D, 0xA66A1E7F),
            new LoginKey("7.0.35 2D",       0x283235CD, 0xA1345E7F),
            new LoginKey("7.0.34 2D",       0x28EAAFDD, 0xA157227F),
            new LoginKey("7.0.33 2D",       0x28A325ED, 0xA1767E7F),
            new LoginKey("7.0.32 2D",       0x289BA7FD, 0xA169527F),
            new LoginKey("7.0.31 2D",       0x295C260D, 0xA197BE7F),
            new LoginKey("7.0.30 2D",       0x2904AC1D, 0xA1BCA27F),
            new LoginKey("7.0.29 2D",       0x29CD362D, 0xA1D59E7F),
            new LoginKey("7.0.28 2D",       0x29B5843D, 0xA1EA127F),
            new LoginKey("7.0.27 2D",       0x2A7E164D, 0xA0081E7F),
            new LoginKey("7.0.26 2D",       0x2A26EC5D, 0xA019A27F),
            new LoginKey("7.0.25 2D",       0x2AEF466D, 0xA07F3E7F),
            new LoginKey("7.0.24 2D",       0x2AD7247D, 0xA065527F),
            new LoginKey("7.0.23 2D",       0x2A9F868D, 0xA0437E7F),
            new LoginKey("7.0.22 2D",       0x2B406C9D, 0xA0A1227F),
            new LoginKey("7.0.21 2D",       0x2B08D6AD, 0xA0875E7F),
            new LoginKey("7.0.20 2D",       0x2BF084BD, 0xA0FD127F),
            new LoginKey("7.0.19 2D",       0x2BB976CD, 0xA0DBDE7F),
            new LoginKey("7.0.18 2D",       0x2C612CDD, 0xA328227F),
            new LoginKey("7.0.17 2D",       0x2C29E6ED, 0xA30EFE7F),
            new LoginKey("7.0.16 2D",       0x2C11A4FD, 0xA313527F),
            new LoginKey("7.0.15 2D",       0x2CDA670D, 0xA3723E7F),
            new LoginKey("7.0.14 2D", 	    0x2C822D1D, 0xA35DA27F),
            new LoginKey("7.0.13 2D", 	    0x2D4AF72D, 0xA3B71E7F),
            new LoginKey("7.0.12 2D", 	    0x2D32853D, 0xA38A127F),
            new LoginKey("7.0.11 2D", 	    0x2DFB574D, 0xA3ED9E7F),
            new LoginKey("7.0.10 2D", 	    0x2DA36D5D, 0xA3C0A27F),
            new LoginKey("7.0.9 2D",		0x2E6B076D, 0xA223BE7F),
            new LoginKey("7.0.8 2D",        0x2E53257D, 0xA23F527F),
            new LoginKey("7.0.7 2D",		0x2E1BC78D, 0xA21BFE7F),
            new LoginKey("7.0.6 2D",        0x2EC3ED9D, 0xA274227F),
            new LoginKey("7.0.5 2D",        0x2E8B97AD, 0xA250DE7F),
            new LoginKey("7.0.4 2D", 		0x2FABA7ED, 0xA2C17E7F),
            new LoginKey("7.0.3 2D", 		0x2FABA7ED, 0xA2C17E7F),
            new LoginKey("7.0.2 2D", 		0x2FABA7ED, 0xA2C17E7F),
            new LoginKey("7.0.1 2D", 	    0x2FABA7ED, 0xA2C17E7F),
            new LoginKey("7.0.0 2D", 		0x2F93A5FD, 0xA2DD527F),
            new LoginKey("6.0.14 2D", 		0x2C022D1D, 0xA31DA27F),
            new LoginKey("6.0.13 2D", 		0x2DCAF72D, 0xA3F71E7F),
            new LoginKey("6.0.12 2D", 		0x2DB2853D, 0xA3CA127F),
            new LoginKey("6.0.11 2D", 		0x2D7B574D, 0xA3AD9E7F),
            new LoginKey("6.0.10 2D", 		0x2D236D5D, 0xA380A27F),
            new LoginKey("6.0.9 2D", 		0x2EEB076D, 0xA263BE7F),
            new LoginKey("6.0.8 2D", 		0x2ED3257D, 0xA27F527F),
            new LoginKey("6.0.7 2D", 		0x2E9BC78D, 0xA25BFE7F),
            new LoginKey("6.0.6 2D", 		0x2E43ED9D, 0xA234227F),
            new LoginKey("6.0.5 2D", 		0x2E0B97AD, 0xA210DE7F),
            new LoginKey("6.0.4 2D", 		0x2FF385BD, 0xA2ED127F),
            new LoginKey("6.0.3 2D", 		0x2FBBB7CD, 0xA2C95E7F),
            new LoginKey("6.0.2 2D", 		0x2F63ADDD, 0xA2A5227F),
            new LoginKey("6.0.1 2D", 		0x2F2BA7ED, 0xA2817E7F),
            new LoginKey("6.0.0 2D", 		0x2f13a5fd, 0xa29d527f),
            new LoginKey("5.0.9 2D", 		0x2F6B076D, 0xA2A3BE7F),
            new LoginKey("5.0.8 2D", 		0x2F53257D, 0xA2BF527F),
            new LoginKey("5.0.7 2D", 		0x10140441, 0xA29BFE7F),
            new LoginKey("5.0.6 2D", 		0x2fc3ed9c, 0xa2f4227f),
            new LoginKey("5.0.5 2D", 		0x2f8b97ac, 0xa2d0de7f),
            new LoginKey("5.0.4 2D", 		0x2e7385bc, 0xa22d127f),
            new LoginKey("5.0.3 2D", 		0x2e3bb7cc, 0xa2095e7f),
            new LoginKey("5.0.2 2D", 		0x2EE3ADDD, 0xA265227F),
            new LoginKey("5.0.1 2D", 		0x2eaba7ec, 0xa2417e7f),
            new LoginKey("5.0.0 2D", 		0x2E93A5FC, 0xA25D527F),
            new LoginKey("4.0.11 2D", 		0x2C7B574C, 0xA32D9E7F),
            new LoginKey("4.0.10 2D", 		0x2C236D5C, 0xA300A27F),
            new LoginKey("4.0.9 2D", 		0x2FEB076C, 0xA2E3BE7F),
            new LoginKey("4.0.8 2D", 		0x2FD3257C, 0xA2FF527F),
            new LoginKey("4.0.7 2D", 		0x2F9BC78D, 0xA2DBFE7F),
            new LoginKey("4.0.6 2D", 		0x2F43ED9C, 0xA2B4227F),
            new LoginKey("4.0.5 2D", 		0x2F0B97AC, 0xA290DE7F),
            new LoginKey("4.0.4 2D", 		0x2EF385BC, 0xA26D127F),
            new LoginKey("4.0.3 2D", 		0x2EBBB7CC, 0xA2495E7F),
            new LoginKey("4.0.2 2D", 		0x2E63ADDC, 0xA225227F),
            new LoginKey("4.0.1 2D", 		0x2E2BA7EC, 0xA2017E7F),
            new LoginKey("4.0.0 2D", 		0x2E13A5FC, 0xA21D527F),
            new LoginKey("3.0.8 2D", 		0x2C53257C, 0xA33F527F),
            new LoginKey("3.0.7 2D", 		0x2C1BC78C, 0xA31BFE7F),
            new LoginKey("3.0.6 2D", 		0x2CC3ED9C, 0xA374227F),
            new LoginKey("3.0.5 2D", 		0x2C8B97AC, 0xA350DE7F),
            new LoginKey("3.0.4 2D", 		0x2D7385BD, 0xA3AD127F),
            new LoginKey("3.0.3 2D", 		0x2D3BB7CD, 0xA3895E7F),
            new LoginKey("3.0.2 2D", 		0x2DE3ADDD, 0xA3E5227F),
            new LoginKey("3.0.1 2D", 		0x2DABA7ED, 0xA3C17E7F),
            new LoginKey("3.0.0 2D", 		0x2D93A5FD, 0xA3DD527F),
		};
    }
}
