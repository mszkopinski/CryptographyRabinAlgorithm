using System;
using System.Security.Cryptography;

namespace RabinEncryption.Lib.Rabin
{
    public class BigNumber
    {
        // zdefiniowanie wszystkich liczb pierwszych mniejszych od 2000
        public static readonly int[] PrimeNumbersBelow2000 = {
           2,    3,    5,    7,   11,   13,   17,   19,   23,   29,   31,   37,   41,   43,   47,   53,   59,   61,   67,   71,
          73,   79,   83,   89,   97,  101,  103,  107,  109,  113,  127,  131,  137,  139,  149,  151,  157,  163,  167,  173,
         179,  181,  191,  193,  197,  199,  211,  223,  227,  229,  233,  239,  241,  251,  257,  263,  269,  271,  277,  281,
         283,  293,  307,  311,  313,  317,  331,  337,  347,  349,  353,  359,  367,  373,  379,  383,  389,  397,  401,  409,
         419,  421,  431,  433,  439,  443,  449,  457,  461,  463,  467,  479,  487,  491,  499,  503,  509,  521,  523,  541,
         547,  557,  563,  569,  571,  577,  587,  593,  599,  601,  607,  613,  617,  619,  631,  641,  643,  647,  653,  659,
         661,  673,  677,  683,  691,  701,  709,  719,  727,  733,  739,  743,  751,  757,  761,  769,  773,  787,  797,  809,
         811,  821,  823,  827,  829,  839,  853,  857,  859,  863,  877,  881,  883,  887,  907,  911,  919,  929,  937,  941,
         947,  953,  967,  971,  977,  983,  991,  997, 1009, 1013, 1019, 1021, 1031, 1033, 1039, 1049, 1051, 1061, 1063, 1069,
        1087, 1091, 1093, 1097, 1103, 1109, 1117, 1123, 1129, 1151, 1153, 1163, 1171, 1181, 1187, 1193, 1201, 1213, 1217, 1223,
        1229, 1231, 1237, 1249, 1259, 1277, 1279, 1283, 1289, 1291, 1297, 1301, 1303, 1307, 1319, 1321, 1327, 1361, 1367, 1373,
        1381, 1399, 1409, 1423, 1427, 1429, 1433, 1439, 1447, 1451, 1453, 1459, 1471, 1481, 1483, 1487, 1489, 1493, 1499, 1511,
        1523, 1531, 1543, 1549, 1553, 1559, 1567, 1571, 1579, 1583, 1597, 1601, 1607, 1609, 1613, 1619, 1621, 1627, 1637, 1657,
        1663, 1667, 1669, 1693, 1697, 1699, 1709, 1721, 1723, 1733, 1741, 1747, 1753, 1759, 1777, 1783, 1787, 1789, 1801, 1811,
        1823, 1831, 1847, 1861, 1867, 1871, 1873, 1877, 1879, 1889, 1901, 1907, 1913, 1931, 1933, 1949, 1951, 1973, 1979, 1987,
        1993, 1997, 1999 };

        // zdefiniowanie stałych używanych przy obliczaniu 3mod4
        public static BigNumber Zero = new BigNumber(0);
        public static BigNumber One = new BigNumber(1);
        public static BigNumber Two = new BigNumber(2);
        public static BigNumber Three = new BigNumber(3);
        public static BigNumber Four = new BigNumber(4);

        // zdefiniowanie maksymalnej długości liczby
        const int MaxLength = 140;

        // licznik wykorzystywany do obliczania długości bloku danych
        public int DataLength;

        // zmienna przetrzymująca blok danych
        private readonly uint[] dataBlock = null;

        // konstruktor domyślny inicjalizujący obiekt klasy DuzaLiczba
        public BigNumber()
        {
            dataBlock = new uint[MaxLength];
            DataLength = 1;
        }

        // konstruktor tworzący obiekt klasy, gdzie wartość jest typu long
        public BigNumber(long value)
        {
            dataBlock = new uint[MaxLength];
            var tempVal = value;

            // kopiowanie bitów z typu long do typu DuzaLiczba bez straty długości danych typu long
            DataLength = 0;
            while (value != 0 && DataLength < MaxLength)
            {
                dataBlock[DataLength] = (uint)(value & 0xFFFFFFFF);
                value >>= 32;
                DataLength++;
            }

            if (tempVal > 0)         // sprawdzenie przepełnienia +ve wartosc
            {
                if (value != 0 || (dataBlock[MaxLength - 1] & 0x80000000) != 0)
                    throw new ArithmeticException("Dodatnie przepełnienie w konstruktorze.");
            }
            else if (tempVal < 0)    // sprawdzenie przepelnienia -ve wartosc
            {
                if (value != -1 || (dataBlock[DataLength - 1] & 0x80000000) == 0)
                    throw new ArithmeticException("Ujemny niedomiar w konstruktorze.");
            }

            if (DataLength == 0)
            {
                DataLength = 1;
            }
        }

        // konstruktor tworzący obiekt klasy, gdzie wartość jest typu ulong
        public BigNumber(ulong value)
        {
            dataBlock = new uint[MaxLength];

            // kopiowanie bitów z typu ulong do typu DuzaLiczba bez straty długości danych typu ulong
            DataLength = 0;
            while (value != 0 && DataLength < MaxLength)
            {
                dataBlock[DataLength] = (uint)(value & 0xFFFFFFFF);
                value >>= 32;
                DataLength++;
            }

            // sprawdzenie przepełnienia +ve wartosc
            if (value != 0 || (dataBlock[MaxLength - 1] & 0x80000000) != 0)
                throw (new ArithmeticException("Dodatnie przepełnienie w konstruktorze."));

            if (DataLength == 0)
                DataLength = 1;
        }

        // konstruktor kopiujący wartości obiektu DuzaLiczba bi do nowego obiektu typu DuzaLiczba
        public BigNumber(BigNumber otherBigNumber)
        {
            dataBlock = new uint[MaxLength];

            DataLength = otherBigNumber.DataLength;

            for (int i = 0; i < DataLength; i++)
                dataBlock[i] = otherBigNumber.dataBlock[i];
        }

        // konstruktor tworzący obiekt klasy, gdzie wartością wejściową jest lista bitów
        public BigNumber(System.Collections.Generic.IList<byte> daneWejsciowe, int dlugosc = -1, int przesuniecie = 0)
        {
            // Jeśli ustawiono dlugosc na -1 to dlugosc wykorzystywana w programie ustawiana jest jako ilość bitów minus przesunięcie. 
            // W przeciwnym przypadku dlugoscWejsciowa ustawiana jest na podaną długość.
            var dlugoscWejsciowa = dlugosc == -1 ? daneWejsciowe.Count - przesuniecie : dlugosc;

            DataLength = dlugoscWejsciowa >> 2;

            int leftOver = dlugoscWejsciowa & 0x3;
            if (leftOver != 0)         // długość nie jest wielokrotnością 4
                DataLength++;

            if (DataLength > MaxLength || dlugoscWejsciowa > daneWejsciowe.Count - przesuniecie)
                throw (new ArithmeticException("Przepełnienie bitów w konstruktorze."));


            dataBlock = new uint[MaxLength];

            for (int i = dlugoscWejsciowa - 1, j = 0; i >= 3; i -= 4, j++)
            {
                dataBlock[j] = (uint)((daneWejsciowe[przesuniecie + i - 3] << 24) + (daneWejsciowe[przesuniecie + i - 2] << 16) +
                                 (daneWejsciowe[przesuniecie + i - 1] << 8) + daneWejsciowe[przesuniecie + i]);
            }

            if (leftOver == 1)
                dataBlock[DataLength - 1] = (uint)daneWejsciowe[przesuniecie + 0];
            else if (leftOver == 2)
                dataBlock[DataLength - 1] = (uint)((daneWejsciowe[przesuniecie + 0] << 8) + daneWejsciowe[przesuniecie + 1]);
            else if (leftOver == 3)
                dataBlock[DataLength - 1] = (uint)((daneWejsciowe[przesuniecie + 0] << 16) + (daneWejsciowe[przesuniecie + 1] << 8) + daneWejsciowe[przesuniecie + 2]);


            if (DataLength == 0)
                DataLength = 1;

            while (DataLength > 1 && dataBlock[DataLength - 1] == 0)
                DataLength--;
        }

        // konstruktor tworzący obiekt klasy, gdzie wartością wejściową jest blok bitów
        public BigNumber(uint[] daneWejsciowe)
        {
            DataLength = daneWejsciowe.Length;

            if (DataLength > MaxLength)
                throw (new ArithmeticException("Przepełnienie bitów w konstruktorze."));

            dataBlock = new uint[MaxLength];

            for (int i = DataLength - 1, j = 0; i >= 0; i--, j++)
                dataBlock[j] = daneWejsciowe[i];

            while (DataLength > 1 && dataBlock[DataLength - 1] == 0)
                DataLength--;
        }

        // definicja metody biorącej liczbę pseudo pierwszą
        public static BigNumber GetPseudoPrime(int bits, int confidence, Random random)
        {
            var result = new BigNumber();
            var isDone = false;

            while (!isDone)
            {
                result.wezLosoweBity(bits, random);
                result.dataBlock[0] |= 0x01;      // zmień na nieparzystą

                // sprawdzenie czy prawdopodobnie pierwsza
                isDone = result.IsLikelyPrime(confidence);
            }
            return result;
        }

        // definicja metody sprawdzającej czy liczba prawdopodobnie jest pierwsza
        public bool IsLikelyPrime(int confidence)
        {
            BigNumber currentBigNumber;
            if ((dataBlock[MaxLength - 1] & 0x80000000) != 0)        // ujemna
                currentBigNumber = -this;
            else
                currentBigNumber = this;

            // test podzielności przez liczby pierwsze < 2000
            foreach (var divisor in PrimeNumbersBelow2000)
            {
                if (divisor >= currentBigNumber) break;

                var result = currentBigNumber % divisor;
                if (result.Intwartosc() == 0)
                    return false;
            }

            return currentBigNumber.RabinMillerTest(confidence);
        }

        // definicja testu Rabina-Millera
        public bool RabinMillerTest(int confidence)
        {
            BigNumber currentBigNumber;
            if ((dataBlock[MaxLength - 1] & 0x80000000) != 0)        // ujemna
                currentBigNumber = -this;
            else
                currentBigNumber = this;

            if (currentBigNumber.DataLength == 1)
            {
                // test małych liczb
                if (currentBigNumber.dataBlock[0] == 0 || currentBigNumber.dataBlock[0] == 1)
                    return false;
                else if (currentBigNumber.dataBlock[0] == 2 || currentBigNumber.dataBlock[0] == 3)
                    return true;
            }

            if ((currentBigNumber.dataBlock[0] & 0x1) == 0)     // parzyste liczby
                return false;


            // oblicza wartość s i t
            BigNumber p_sub1 = currentBigNumber - (new BigNumber(1));
            int s = 0;

            for (int index = 0; index < p_sub1.DataLength; index++)
            {
                uint maska = 0x01;

                for (int i = 0; i < 32; i++)
                {
                    if ((p_sub1.dataBlock[index] & maska) != 0)
                    {
                        index = p_sub1.DataLength;      // do przerwania zewnętrznej pętli
                        break;
                    }
                    maska <<= 1;
                    s++;
                }
            }

            BigNumber t = p_sub1 >> s;

            int bity = currentBigNumber.CountBytes();
            BigNumber a = new BigNumber();
            Random losowe = new Random();

            for (int round = 0; round < confidence; round++)
            {
                bool gotowe = false;

                while (!gotowe)     // stwórz a < n
                {
                    int testBits = 0;

                    // sprawdzenie czy "a" ma najmniejsze 2 bity
                    while (testBits < 2)
                        testBits = (int)(losowe.NextDouble() * bity);

                    a.wezLosoweBity(testBits, losowe);

                    int byteLen = a.DataLength;

                    // sprawdzenie czy "a" nei jest 0
                    if (byteLen > 1 || (byteLen == 1 && a.dataBlock[0] != 1))
                        gotowe = true;
                }

                // sprawdzenie czy istnieje czynnik
                BigNumber nwdTest = a.nwd(currentBigNumber);
                if (nwdTest.DataLength == 1 && nwdTest.dataBlock[0] != 1)
                    return false;

                BigNumber b = a.ModPow(t, currentBigNumber);

                bool wynik = false;

                if (b.DataLength == 1 && b.dataBlock[0] == 1)         // a^t mod p = 1
                    wynik = true;

                for (int j = 0; wynik == false && j < s; j++)
                {
                    if (b == p_sub1)         // a^((2^j)*t) mod p = p-1 for some 0 <= j <= s-1
                    {
                        wynik = true;
                        break;
                    }

                    b = (b * b) % currentBigNumber;
                }

                if (wynik == false)
                    return false;
            }
            return true;
        }

        public static implicit operator BigNumber(long wartosc) => (new BigNumber(wartosc));
        public static implicit operator BigNumber(ulong wartosc) => (new BigNumber(wartosc));
        public static implicit operator BigNumber(int wartosc) => (new BigNumber((long)wartosc));
        public static implicit operator BigNumber(uint wartosc) => (new BigNumber((ulong)wartosc));

        //definicja operacji dodawania obiektów klasy DuzaLiczba
        public static BigNumber operator +(BigNumber liczba1, BigNumber liczba2)
        {
            BigNumber wynik = new BigNumber()
            {
                DataLength = (liczba1.DataLength > liczba2.DataLength) ? liczba1.DataLength : liczba2.DataLength
            };

            long nosnik = 0;
            for (int i = 0; i < wynik.DataLength; i++)
            {
                long sum = (long)liczba1.dataBlock[i] + (long)liczba2.dataBlock[i] + nosnik;
                nosnik = sum >> 32;
                wynik.dataBlock[i] = (uint)(sum & 0xFFFFFFFF);
            }

            if (nosnik != 0 && wynik.DataLength < MaxLength)
            {
                wynik.dataBlock[wynik.DataLength] = (uint)(nosnik);
                wynik.DataLength++;
            }

            while (wynik.DataLength > 1 && wynik.dataBlock[wynik.DataLength - 1] == 0)
                wynik.DataLength--;


            // sprawdzenie przepełnienia
            int ostatniaPozycja = MaxLength - 1;
            if ((liczba1.dataBlock[ostatniaPozycja] & 0x80000000) == (liczba2.dataBlock[ostatniaPozycja] & 0x80000000) &&
               (wynik.dataBlock[ostatniaPozycja] & 0x80000000) != (liczba1.dataBlock[ostatniaPozycja] & 0x80000000))
            {
                throw (new ArithmeticException());
            }

            return wynik;
        }

        //definicja operacji odejmowania obiektów klasy DuzaLiczba
        public static BigNumber operator -(BigNumber liczba1, BigNumber liczba2)
        {
            BigNumber wynik = new BigNumber()
            {
                DataLength = (liczba1.DataLength > liczba2.DataLength) ? liczba1.DataLength : liczba2.DataLength
            };

            long nosnik = 0;
            for (int i = 0; i < wynik.DataLength; i++)
            {
                long roznica;

                roznica = (long)liczba1.dataBlock[i] - (long)liczba2.dataBlock[i] - nosnik;
                wynik.dataBlock[i] = (uint)(roznica & 0xFFFFFFFF);

                if (roznica < 0)
                    nosnik = 1;
                else
                    nosnik = 0;
            }

            // zmiana na negatywną wartość
            if (nosnik != 0)
            {
                for (int i = wynik.DataLength; i < MaxLength; i++)
                    wynik.dataBlock[i] = 0xFFFFFFFF;
                wynik.DataLength = MaxLength;
            }

            // dla a - (-b)
            while (wynik.DataLength > 1 && wynik.dataBlock[wynik.DataLength - 1] == 0)
                wynik.DataLength--;

            // sprawdzenie przepełnienia
            int ostatniaPozycja = MaxLength - 1;
            if ((liczba1.dataBlock[ostatniaPozycja] & 0x80000000) != (liczba2.dataBlock[ostatniaPozycja] & 0x80000000) &&
               (wynik.dataBlock[ostatniaPozycja] & 0x80000000) != (liczba1.dataBlock[ostatniaPozycja] & 0x80000000))
            {
                throw (new ArithmeticException());
            }

            return wynik;
        }

        //definicja operacji mnożenia obiektów klasy DuzaLiczba
        public static BigNumber operator *(BigNumber liczba1, BigNumber liczba2)
        {
            int ostatniaPozycja = MaxLength - 1;
            bool liczba1Ujemna = false, liczba2Ujemna = false;

            // wartość bezwzględna z liczb
            try
            {
                if ((liczba1.dataBlock[ostatniaPozycja] & 0x80000000) != 0)     // liczba1 ujemna
                {
                    liczba1Ujemna = true; liczba1 = -liczba1;
                }
                if ((liczba2.dataBlock[ostatniaPozycja] & 0x80000000) != 0)     // liczba2 ujemna
                {
                    liczba2Ujemna = true; liczba2 = -liczba2;
                }
            }
            catch (Exception) { }

            BigNumber wynik = new BigNumber();

            // wymnożenie wartości bezwzględnych
            try
            {
                for (int i = 0; i < liczba1.DataLength; i++)
                {
                    if (liczba1.dataBlock[i] == 0) continue;

                    ulong nosnik = 0;
                    for (int j = 0, k = i; j < liczba2.DataLength; j++, k++)
                    {
                        // k = i + j
                        ulong wartosc = ((ulong)liczba1.dataBlock[i] * (ulong)liczba2.dataBlock[j]) +
                                     (ulong)wynik.dataBlock[k] + nosnik;

                        wynik.dataBlock[k] = (uint)(wartosc & 0xFFFFFFFF);
                        nosnik = (wartosc >> 32);
                    }

                    if (nosnik != 0)
                        wynik.dataBlock[i + liczba2.DataLength] = (uint)nosnik;
                }
            }
            catch (Exception)
            {
                throw (new ArithmeticException("Przepełnienie operacji mnożenia."));
            }


            wynik.DataLength = liczba1.DataLength + liczba2.DataLength;
            if (wynik.DataLength > MaxLength)
                wynik.DataLength = MaxLength;

            while (wynik.DataLength > 1 && wynik.dataBlock[wynik.DataLength - 1] == 0)
                wynik.DataLength--;

            // sprawdzenie przepełnienia (wynik jest -ve)
            if ((wynik.dataBlock[ostatniaPozycja] & 0x80000000) != 0)
            {
                if (liczba1Ujemna != liczba2Ujemna && wynik.dataBlock[ostatniaPozycja] == 0x80000000)    // znak różnicy
                {
                    // obsługa przypadku kiedy mnożenie daje maksymalną liczbę ujemną

                    if (wynik.DataLength == 1)
                        return wynik;
                    else
                    {
                        bool jestMaksymalnaUjemna = true;
                        for (int i = 0; i < wynik.DataLength - 1 && jestMaksymalnaUjemna; i++)
                        {
                            if (wynik.dataBlock[i] != 0)
                                jestMaksymalnaUjemna = false;
                        }

                        if (jestMaksymalnaUjemna)
                            return wynik;
                    }
                }

                throw (new ArithmeticException("Przepełnienie operacji mnożenia."));
            }

            // jeżeli różnica znaków jest ujemna
            if (liczba1Ujemna != liczba2Ujemna)
                return -wynik;

            return wynik;
        }

        // definicja operacji przesunięcia bitowego w lewo
        public static BigNumber operator <<(BigNumber liczba1, int wartoscPrzesuniecia)
        {
            BigNumber wynik = new BigNumber(liczba1);
            wynik.DataLength = przesuniecieWLewo(wynik.dataBlock, wartoscPrzesuniecia);

            return wynik;
        }

        // najmniej znaczące bity w dolnej części bufora
        private static int przesuniecieWLewo(uint[] bufor, int wartoscPrzesuniecia)
        {
            int licznikPrzesuniecia = 32;
            int dlugoscBufora = bufor.Length;

            while (dlugoscBufora > 1 && bufor[dlugoscBufora - 1] == 0)
                dlugoscBufora--;

            for (int licznik = wartoscPrzesuniecia; licznik > 0;)
            {
                if (licznik < licznikPrzesuniecia)
                    licznikPrzesuniecia = licznik;

                ulong nosnik = 0;
                for (int i = 0; i < dlugoscBufora; i++)
                {
                    ulong wartosc = ((ulong)bufor[i]) << licznikPrzesuniecia;
                    wartosc |= nosnik;

                    bufor[i] = (uint)(wartosc & 0xFFFFFFFF);
                    nosnik = wartosc >> 32;
                }

                if (nosnik != 0)
                {
                    if (dlugoscBufora + 1 <= bufor.Length)
                    {
                        bufor[dlugoscBufora] = (uint)nosnik;
                        dlugoscBufora++;
                    }
                }
                licznik -= licznikPrzesuniecia;
            }
            return dlugoscBufora;
        }

        // definicja przesunięcia bitowego w prawo
        public static BigNumber operator >>(BigNumber liczba1, int wartoscPrzesuniecia)
        {
            BigNumber wynik = new BigNumber(liczba1);
            wynik.DataLength = przesuniecieWPrawo(wynik.dataBlock, wartoscPrzesuniecia);


            if ((liczba1.dataBlock[MaxLength - 1] & 0x80000000) != 0) // ujemna
            {
                for (int i = MaxLength - 1; i >= wynik.DataLength; i--)
                    wynik.dataBlock[i] = 0xFFFFFFFF;

                uint maska = 0x80000000;
                for (int i = 0; i < 32; i++)
                {
                    if ((wynik.dataBlock[wynik.DataLength - 1] & maska) != 0)
                        break;

                    wynik.dataBlock[wynik.DataLength - 1] |= maska;
                    maska >>= 1;
                }
                wynik.DataLength = MaxLength;
            }

            return wynik;
        }


        private static int przesuniecieWPrawo(uint[] bufor, int wartoscPrzesuniecia)
        {
            int licznikPrzesuniecia = 32;
            int invShift = 0;
            int dlugoscBufora = bufor.Length;

            while (dlugoscBufora > 1 && bufor[dlugoscBufora - 1] == 0)
                dlugoscBufora--;

            for (int licznik = wartoscPrzesuniecia; licznik > 0;)
            {
                if (licznik < licznikPrzesuniecia)
                {
                    licznikPrzesuniecia = licznik;
                    invShift = 32 - licznikPrzesuniecia;
                }

                ulong nosnik = 0;
                for (int i = dlugoscBufora - 1; i >= 0; i--)
                {
                    ulong wartosc = ((ulong)bufor[i]) >> licznikPrzesuniecia;
                    wartosc |= nosnik;

                    nosnik = (((ulong)bufor[i]) << invShift) & 0xFFFFFFFF;
                    bufor[i] = (uint)(wartosc);
                }

                licznik -= licznikPrzesuniecia;
            }

            while (dlugoscBufora > 1 && bufor[dlugoscBufora - 1] == 0)
                dlugoscBufora--;

            return dlugoscBufora;
        }

        // definicja operacji negacji obiektu klasy DuzaLiczba 
        public static BigNumber operator -(BigNumber liczba1)
        {
            // obsługa negacji zera
            if (liczba1.DataLength == 1 && liczba1.dataBlock[0] == 0)
                return (new BigNumber());

            BigNumber wynik = new BigNumber(liczba1);

            // pierwsze uzupełnienie
            for (int i = 0; i < MaxLength; i++)
                wynik.dataBlock[i] = (uint)(~(liczba1.dataBlock[i]));

            // dodaj jeden do wyniku pierwszego uzupełnienia
            long wartosc, nosnik = 1;
            int index = 0;

            while (nosnik != 0 && index < MaxLength)
            {
                wartosc = (long)(wynik.dataBlock[index]);
                wartosc++;

                wynik.dataBlock[index] = (uint)(wartosc & 0xFFFFFFFF);
                nosnik = wartosc >> 32;

                index++;
            }

            if ((liczba1.dataBlock[MaxLength - 1] & 0x80000000) == (wynik.dataBlock[MaxLength - 1] & 0x80000000))
                throw (new ArithmeticException("Przepełnienie w negacji.\n"));

            wynik.DataLength = MaxLength;

            while (wynik.DataLength > 1 && wynik.dataBlock[wynik.DataLength - 1] == 0)
                wynik.DataLength--;
            return wynik;
        }

        // definicja operacji porównania obiektów klasy DuzaLiczba
        public static bool operator ==(BigNumber liczba1, BigNumber liczba2) => liczba1.Equals(liczba2);

        // definicja operacji negacji porównania obiektów klasy DuzaLiczba
        public static bool operator !=(BigNumber liczba1, BigNumber liczba2) => !(liczba1.Equals(liczba2));

        // definicja metody porównującej obiekty klasy DuzaLiczba
        public override bool Equals(object o)
        {
            BigNumber bi = (BigNumber)o;

            if (this.DataLength != bi.DataLength)
                return false;

            for (int i = 0; i < this.DataLength; i++)
            {
                if (this.dataBlock[i] != bi.dataBlock[i])
                    return false;
            }
            return true;
        }

        public override int GetHashCode() => ToString().GetHashCode();

        //definicja operacji nierówności między dwoma obiektami klasy DuzaLiczba
        public static bool operator >(BigNumber liczba1, BigNumber liczba2)
        {
            int pozycja = MaxLength - 1;

            // liczba1 jest ujemna, liczba2 jest dodatnia
            if ((liczba1.dataBlock[pozycja] & 0x80000000) != 0 && (liczba2.dataBlock[pozycja] & 0x80000000) == 0)
                return false;

            // liczba1 jest dodatnia, liczba2 jest ujemna
            else if ((liczba1.dataBlock[pozycja] & 0x80000000) == 0 && (liczba2.dataBlock[pozycja] & 0x80000000) != 0)
                return true;

            // te same znaki
            int dlugosc = (liczba1.DataLength > liczba2.DataLength) ? liczba1.DataLength : liczba2.DataLength;
            for (pozycja = dlugosc - 1; pozycja >= 0 && liczba1.dataBlock[pozycja] == liczba2.dataBlock[pozycja]; pozycja--) ;

            if (pozycja >= 0)
            {
                if (liczba1.dataBlock[pozycja] > liczba2.dataBlock[pozycja])
                    return true;
                return false;
            }
            return false;
        }

        //definicja operacji nierówności między dwoma obiektami klasy DuzaLiczba
        public static bool operator <(BigNumber liczba1, BigNumber liczba2)
        {
            int pozycja = MaxLength - 1;

            // liczba1 jest ujemna, liczba2 jest dodatnia
            if ((liczba1.dataBlock[pozycja] & 0x80000000) != 0 && (liczba2.dataBlock[pozycja] & 0x80000000) == 0)
                return true;

            // liczba1 jest dodatnia, liczba2 jest ujemna
            else if ((liczba1.dataBlock[pozycja] & 0x80000000) == 0 && (liczba2.dataBlock[pozycja] & 0x80000000) != 0)
                return false;

            // te same znaki
            int dlugosc = (liczba1.DataLength > liczba2.DataLength) ? liczba1.DataLength : liczba2.DataLength;
            for (pozycja = dlugosc - 1; pozycja >= 0 && liczba1.dataBlock[pozycja] == liczba2.dataBlock[pozycja]; pozycja--) ;

            if (pozycja >= 0)
            {
                if (liczba1.dataBlock[pozycja] < liczba2.dataBlock[pozycja])
                    return true;
                return false;
            }
            return false;
        }

        // definicja operacji nierówności ostrych na obiektach klasy DuzaLiczba
        public static bool operator >=(BigNumber liczba1, BigNumber liczba2) => (liczba1 == liczba2 || liczba1 > liczba2);
        public static bool operator <=(BigNumber liczba1, BigNumber liczba2) => (liczba1 == liczba2 || liczba1 < liczba2);

        private static void dzielenieBitoweZlozone(BigNumber liczba1, BigNumber liczba2,
                                            BigNumber ilorazDzielenia, BigNumber resztaDzielenia)
        {
            uint[] wynik = new uint[MaxLength];

            int dlugoscReszty = liczba1.DataLength + 1;
            uint[] reszta = new uint[dlugoscReszty];

            uint maska = 0x80000000;
            uint wartosc = liczba2.dataBlock[liczba2.DataLength - 1];
            int przesuniecie = 0, wynikPozycja = 0;

            while (maska != 0 && (wartosc & maska) == 0)
            {
                przesuniecie++; maska >>= 1;
            }

            for (int i = 0; i < liczba1.DataLength; i++)
                reszta[i] = liczba1.dataBlock[i];
            przesuniecieWLewo(reszta, przesuniecie);
            liczba2 = liczba2 << przesuniecie;

            int j = dlugoscReszty - liczba2.DataLength;
            int pozycja = dlugoscReszty - 1;

            ulong pierwszyBajtDzielnika = liczba2.dataBlock[liczba2.DataLength - 1];
            ulong drugiBajtDzielnika = liczba2.dataBlock[liczba2.DataLength - 2];

            int dlugoscDzielnika = liczba2.DataLength + 1;
            uint[] dzielonaCzesc = new uint[dlugoscDzielnika];

            while (j > 0)
            {
                ulong dzielna = ((ulong)reszta[pozycja] << 32) + (ulong)reszta[pozycja - 1];

                ulong q_hat = dzielna / pierwszyBajtDzielnika;
                ulong r_hat = dzielna % pierwszyBajtDzielnika;

                bool gotowe = false;
                while (!gotowe)
                {
                    gotowe = true;

                    if (q_hat == 0x100000000 ||
                       (q_hat * drugiBajtDzielnika) > ((r_hat << 32) + reszta[pozycja - 2]))
                    {
                        q_hat--;
                        r_hat += pierwszyBajtDzielnika;

                        if (r_hat < 0x100000000)
                            gotowe = false;
                    }
                }

                for (int h = 0; h < dlugoscDzielnika; h++)
                    dzielonaCzesc[h] = reszta[pozycja - h];

                BigNumber kk = new BigNumber(dzielonaCzesc);
                BigNumber ss = liczba2 * (long)q_hat;

                while (ss > kk)
                {
                    q_hat--;
                    ss -= liczba2;
                }
                BigNumber yy = kk - ss;

                for (int h = 0; h < dlugoscDzielnika; h++)
                    reszta[pozycja - h] = yy.dataBlock[liczba2.DataLength - h];

                wynik[wynikPozycja++] = (uint)q_hat;

                pozycja--;
                j--;
            }

            ilorazDzielenia.DataLength = wynikPozycja;
            int y = 0;
            for (int x = ilorazDzielenia.DataLength - 1; x >= 0; x--, y++)
                ilorazDzielenia.dataBlock[y] = wynik[x];
            for (; y < MaxLength; y++)
                ilorazDzielenia.dataBlock[y] = 0;

            while (ilorazDzielenia.DataLength > 1 && ilorazDzielenia.dataBlock[ilorazDzielenia.DataLength - 1] == 0)
                ilorazDzielenia.DataLength--;

            if (ilorazDzielenia.DataLength == 0)
                ilorazDzielenia.DataLength = 1;

            resztaDzielenia.DataLength = przesuniecieWPrawo(reszta, przesuniecie);

            for (y = 0; y < resztaDzielenia.DataLength; y++)
                resztaDzielenia.dataBlock[y] = reszta[y];
            for (; y < MaxLength; y++)
                resztaDzielenia.dataBlock[y] = 0;
        }

        private static void dzielenieBitowePojedyncze(BigNumber liczba1, BigNumber liczba2,
                                             BigNumber ilorazDzielenia, BigNumber resztaDzielenia)
        {
            uint[] wynik = new uint[MaxLength];
            int wynikPozycja = 0;

            // kopiowanie dzielnej do reszty
            for (int i = 0; i < MaxLength; i++)
                resztaDzielenia.dataBlock[i] = liczba1.dataBlock[i];
            resztaDzielenia.DataLength = liczba1.DataLength;

            while (resztaDzielenia.DataLength > 1 && resztaDzielenia.dataBlock[resztaDzielenia.DataLength - 1] == 0)
                resztaDzielenia.DataLength--;

            ulong dzielnik = (ulong)liczba2.dataBlock[0];
            int pozycja = resztaDzielenia.DataLength - 1;
            ulong dzielna = (ulong)resztaDzielenia.dataBlock[pozycja];

            if (dzielna >= dzielnik)
            {
                ulong iloraz = dzielna / dzielnik;
                wynik[wynikPozycja++] = (uint)iloraz;

                resztaDzielenia.dataBlock[pozycja] = (uint)(dzielna % dzielnik);
            }
            pozycja--;

            while (pozycja >= 0)
            {
                dzielna = ((ulong)resztaDzielenia.dataBlock[pozycja + 1] << 32) + (ulong)resztaDzielenia.dataBlock[pozycja];
                ulong iloraz = dzielna / dzielnik;
                wynik[wynikPozycja++] = (uint)iloraz;

                resztaDzielenia.dataBlock[pozycja + 1] = 0;
                resztaDzielenia.dataBlock[pozycja--] = (uint)(dzielna % dzielnik);
            }

            ilorazDzielenia.DataLength = wynikPozycja;
            int j = 0;
            for (int i = ilorazDzielenia.DataLength - 1; i >= 0; i--, j++)
                ilorazDzielenia.dataBlock[j] = wynik[i];
            for (; j < MaxLength; j++)
                ilorazDzielenia.dataBlock[j] = 0;

            while (ilorazDzielenia.DataLength > 1 && ilorazDzielenia.dataBlock[ilorazDzielenia.DataLength - 1] == 0)
                ilorazDzielenia.DataLength--;

            if (ilorazDzielenia.DataLength == 0)
                ilorazDzielenia.DataLength = 1;

            while (resztaDzielenia.DataLength > 1 && resztaDzielenia.dataBlock[resztaDzielenia.DataLength - 1] == 0)
                resztaDzielenia.DataLength--;
        }

        // definicja operacji dzielenia dwóch obiektów klasy DuzaLiczba
        public static BigNumber operator /(BigNumber liczba1, BigNumber liczba2)
        {
            BigNumber iloraz = new BigNumber();
            BigNumber reszta = new BigNumber();

            int ostatniaPozycja = MaxLength - 1;
            bool dzielnikUjemny = false, dzielnaUjemna = false;

            if ((liczba1.dataBlock[ostatniaPozycja] & 0x80000000) != 0)     // liczba1 ujemna
            {
                liczba1 = -liczba1;
                dzielnaUjemna = true;
            }
            if ((liczba2.dataBlock[ostatniaPozycja] & 0x80000000) != 0)     // liczba2 ujemna
            {
                liczba2 = -liczba2;
                dzielnikUjemny = true;
            }

            if (liczba1 < liczba2)
            {
                return iloraz;
            }

            else
            {
                if (liczba2.DataLength == 1)
                    dzielenieBitowePojedyncze(liczba1, liczba2, iloraz, reszta);
                else
                    dzielenieBitoweZlozone(liczba1, liczba2, iloraz, reszta);

                if (dzielnaUjemna != dzielnikUjemny)
                    return -iloraz;

                return iloraz;
            }
        }

        //definicja operacji modulo obiektów klasy DuzaLiczba
        public static BigNumber operator %(BigNumber liczba1, BigNumber liczba2)
        {
            BigNumber iloraz = new BigNumber();
            BigNumber reszta = new BigNumber(liczba1);

            int ostatniaPozycja = MaxLength - 1;
            bool dzielnaUjemna = false;

            if ((liczba1.dataBlock[ostatniaPozycja] & 0x80000000) != 0)     // liczba1 ujemna
            {
                liczba1 = -liczba1;
                dzielnaUjemna = true;
            }
            if ((liczba2.dataBlock[ostatniaPozycja] & 0x80000000) != 0)     // liczba2 ujemna
                liczba2 = -liczba2;

            if (liczba1 < liczba2)
            {
                return reszta;
            }

            else
            {
                if (liczba2.DataLength == 1)
                    dzielenieBitowePojedyncze(liczba1, liczba2, iloraz, reszta);
                else
                    dzielenieBitoweZlozone(liczba1, liczba2, iloraz, reszta);

                if (dzielnaUjemna)
                    return -reszta;

                return reszta;
            }
        }

        public override string ToString() => ToString(10);

        // definicja metody toString()
        public string ToString(int radix)
        {
            if (radix < 2 || radix > 36)
                throw (new ArgumentException("Radix musi być >= 2 i <= 36"));

            string zestawZnakow = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string wynik = "";

            BigNumber a = this;

            bool ujemna = false;
            if ((a.dataBlock[MaxLength - 1] & 0x80000000) != 0)
            {
                ujemna = true;
                try
                {
                    a = -a;
                }
                catch (Exception) { }
            }

            BigNumber iloraz = new BigNumber();
            BigNumber reszta = new BigNumber();
            BigNumber biRadix = new BigNumber(radix);

            if (a.DataLength == 1 && a.dataBlock[0] == 0)
                wynik = "0";
            else
            {
                while (a.DataLength > 1 || (a.DataLength == 1 && a.dataBlock[0] != 0))
                {
                    dzielenieBitowePojedyncze(a, biRadix, iloraz, reszta);

                    if (reszta.dataBlock[0] < 10)
                        wynik = reszta.dataBlock[0] + wynik;
                    else
                        wynik = zestawZnakow[(int)reszta.dataBlock[0] - 10] + wynik;

                    a = iloraz;
                }
                if (ujemna)
                    wynik = "-" + wynik;
            }

            return wynik;
        }

        // definicja operacji potęgowania modulo
        public BigNumber ModPow(BigNumber exponential, BigNumber n)
        {
            if ((exponential.dataBlock[MaxLength - 1] & 0x80000000) != 0)
                throw (new ArithmeticException("Positive exponents only."));

            BigNumber liczbaWynikowa = 1;
            BigNumber liczbaPomocnicza;
            bool jestUjemne = false;

            if ((this.dataBlock[MaxLength - 1] & 0x80000000) != 0)   // jest ujemne
            {
                liczbaPomocnicza = -this % n;
                jestUjemne = true;
            }
            else
                liczbaPomocnicza = this % n;  // zapewnia (liczbaPomocnicza * liczbaPomocnicza) < b^(2k)

            if ((n.dataBlock[MaxLength - 1] & 0x80000000) != 0)   // ujemna n
                n = -n;

            // oblicza stala = b^(2k) / m
            BigNumber stala = new BigNumber();

            int i = n.DataLength << 1;
            stala.dataBlock[i] = 0x00000001;
            stala.DataLength = i + 1;

            stala = stala / n;
            int wszystkieBity = exponential.CountBytes();
            int licznik = 0;

            // wykonać operację pierwiastkowania i mnożenia potęgowania
            for (int pozycja = 0; pozycja < exponential.DataLength; pozycja++)
            {
                uint maska = 0x01;

                for (int index = 0; index < 32; index++)
                {
                    if ((exponential.dataBlock[pozycja] & maska) != 0)
                        liczbaWynikowa = RedukcjaBarretta(liczbaWynikowa * liczbaPomocnicza, n, stala);

                    maska <<= 1;

                    liczbaPomocnicza = RedukcjaBarretta(liczbaPomocnicza * liczbaPomocnicza, n, stala);


                    if (liczbaPomocnicza.DataLength == 1 && liczbaPomocnicza.dataBlock[0] == 1)
                    {
                        if (jestUjemne && (exponential.dataBlock[0] & 0x1) != 0)    //odd exp
                            return -liczbaWynikowa;
                        return liczbaWynikowa;
                    }
                    licznik++;
                    if (licznik == wszystkieBity)
                        break;
                }
            }

            if (jestUjemne && (exponential.dataBlock[0] & 0x1) != 0)    //odd exp
                return -liczbaWynikowa;

            return liczbaWynikowa;
        }

        //definicja metody przeprowadzającej redukcję Barretta
        private BigNumber RedukcjaBarretta(BigNumber x, BigNumber n, BigNumber stala)
        {
            int k = n.DataLength,
                kPlusJeden = k + 1,
                kMinusJeden = k - 1;

            BigNumber q1 = new BigNumber();

            // q1 = x / b^(k-1)
            for (int i = kMinusJeden, j = 0; i < x.DataLength; i++, j++)
                q1.dataBlock[j] = x.dataBlock[i];
            q1.DataLength = x.DataLength - kMinusJeden;
            if (q1.DataLength <= 0)
                q1.DataLength = 1;


            BigNumber q2 = q1 * stala;
            BigNumber q3 = new BigNumber();

            // q3 = q2 / b^(k+1)
            for (int i = kPlusJeden, j = 0; i < q2.DataLength; i++, j++)
                q3.dataBlock[j] = q2.dataBlock[i];
            q3.DataLength = q2.DataLength - kPlusJeden;
            if (q3.DataLength <= 0)
                q3.DataLength = 1;


            // r1 = x mod b^(k+1)
            // zachowaj najniższe (k+1) słowa
            BigNumber r1 = new BigNumber();
            int dlugoscToCopy = (x.DataLength > kPlusJeden) ? kPlusJeden : x.DataLength;
            for (int i = 0; i < dlugoscToCopy; i++)
                r1.dataBlock[i] = x.dataBlock[i];
            r1.DataLength = dlugoscToCopy;


            // r2 = (q3 * n) mod b^(k+1)
            // częściowe mnożenie q3 i n

            BigNumber r2 = new BigNumber();
            for (int i = 0; i < q3.DataLength; i++)
            {
                if (q3.dataBlock[i] == 0) continue;

                ulong nosnik = 0;
                int t = i;
                for (int j = 0; j < n.DataLength && t < kPlusJeden; j++, t++)
                {
                    // t = i + j
                    ulong wartosc = ((ulong)q3.dataBlock[i] * (ulong)n.dataBlock[j]) +
                                 (ulong)r2.dataBlock[t] + nosnik;

                    r2.dataBlock[t] = (uint)(wartosc & 0xFFFFFFFF);
                    nosnik = (wartosc >> 32);
                }

                if (t < kPlusJeden)
                    r2.dataBlock[t] = (uint)nosnik;
            }
            r2.DataLength = kPlusJeden;
            while (r2.DataLength > 1 && r2.dataBlock[r2.DataLength - 1] == 0)
                r2.DataLength--;

            r1 -= r2;
            if ((r1.dataBlock[MaxLength - 1] & 0x80000000) != 0)        // ujemna
            {
                BigNumber wartosc = new BigNumber();
                wartosc.dataBlock[kPlusJeden] = 0x00000001;
                wartosc.DataLength = kPlusJeden + 1;
                r1 += wartosc;
            }

            while (r1 >= n)
                r1 -= n;

            return r1;
        }

        // definicja metody liczącej największy wspólny dzielnik
        public BigNumber nwd(BigNumber bi)
        {
            BigNumber x;
            BigNumber y;

            if ((dataBlock[MaxLength - 1] & 0x80000000) != 0)     // ujemna
                x = -this;
            else
                x = this;

            if ((bi.dataBlock[MaxLength - 1] & 0x80000000) != 0)     // ujemna
                y = -bi;
            else
                y = bi;

            BigNumber g = y;

            while (x.DataLength > 1 || (x.DataLength == 1 && x.dataBlock[0] != 0))
            {
                g = x;
                x = y % x;
                y = g;
            }

            return g;
        }

        // definicja metody losującej bity
        public void wezLosoweBity(int bity, Random losowe)
        {
            int dwords = bity >> 5;
            int remBits = bity & 0x1F;

            if (remBits != 0)
                dwords++;

            if (dwords > MaxLength || bity <= 0)
                throw (new ArithmeticException("Liczba wymagalnych bitów nie jest wartościowalna."));

            byte[] losoweBity = new byte[dwords * 4];
            losowe.NextBytes(losoweBity);

            for (int i = 0; i < dwords; i++)
                dataBlock[i] = BitConverter.ToUInt32(losoweBity, i * 4);

            for (int i = dwords; i < MaxLength; i++)
                dataBlock[i] = 0;

            if (remBits != 0)
            {
                uint maska;

                if (bity != 1)
                {
                    maska = (uint)(0x01 << (remBits - 1));
                    dataBlock[dwords - 1] |= maska;
                }

                maska = (uint)(0xFFFFFFFF >> (32 - remBits));
                dataBlock[dwords - 1] &= maska;
            }
            else
                dataBlock[dwords - 1] |= 0x80000000;

            DataLength = dwords;

            if (DataLength == 0)
                DataLength = 1;
        }

        // definicja metody losującej bity
        public void wezLosoweBity(int bity, RNGCryptoServiceProvider rng)
        {
            int dwords = bity >> 5;
            int remBits = bity & 0x1F;

            if (remBits != 0)
                dwords++;

            if (dwords > MaxLength || bity <= 0)
                throw (new ArithmeticException("Liczba wymagalnych bitów nie jest wartościowalna."));

            byte[] randomBytes = new byte[dwords * 4];
            rng.GetBytes(randomBytes);

            for (int i = 0; i < dwords; i++)
                dataBlock[i] = BitConverter.ToUInt32(randomBytes, i * 4);

            for (int i = dwords; i < MaxLength; i++)
                dataBlock[i] = 0;

            if (remBits != 0)
            {
                uint maska;

                if (bity != 1)
                {
                    maska = (uint)(0x01 << (remBits - 1));
                    dataBlock[dwords - 1] |= maska;
                }

                maska = (uint)(0xFFFFFFFF >> (32 - remBits));
                dataBlock[dwords - 1] &= maska;
            }
            else
                dataBlock[dwords - 1] |= 0x80000000;

            DataLength = dwords;

            if (DataLength == 0)
                DataLength = 1;
        }

        // definicja metody zliczającej bity
        public int CountBytes()
        {
            while (DataLength > 1 && dataBlock[DataLength - 1] == 0)
                DataLength--;

            uint wartosc = dataBlock[DataLength - 1];
            uint maska = 0x80000000;
            int bity = 32;

            while (bity > 0 && (wartosc & maska) == 0)
            {
                bity--;
                maska >>= 1;
            }
            bity += ((DataLength - 1) << 5);

            return bity == 0 ? 1 : bity;
        }






        public int Intwartosc()
        {
            return (int)dataBlock[0];
        }

        // definicja Jacobi
        public static int Jacobi(BigNumber a, BigNumber b)
        {
            // Jacobi zdefiniowane tylko dla nieparzystych liczb całkowitych
            if ((b.dataBlock[0] & 0x1) == 0)
                throw (new ArgumentException("Jacobi zdefiniowane tylko dla nieparzystych liczb całkowitych."));

            if (a >= b) a %= b;
            if (a.DataLength == 1 && a.dataBlock[0] == 0) return 0;  // a == 0
            if (a.DataLength == 1 && a.dataBlock[0] == 1) return 1;  // a == 1

            if (a < 0)
            {
                if ((((b - 1).dataBlock[0]) & 0x2) == 0)       //if( (((b-1) >> 1).dane[0] & 0x1) == 0)
                    return Jacobi(-a, b);
                else
                    return -Jacobi(-a, b);
            }

            int e = 0;
            for (int index = 0; index < a.DataLength; index++)
            {
                uint maska = 0x01;

                for (int i = 0; i < 32; i++)
                {
                    if ((a.dataBlock[index] & maska) != 0)
                    {
                        index = a.DataLength;      // do przerwania zewnętrznej pętli
                        break;
                    }
                    maska <<= 1;
                    e++;
                }
            }

            BigNumber a1 = a >> e;

            int s = 1;
            if ((e & 0x1) != 0 && ((b.dataBlock[0] & 0x7) == 3 || (b.dataBlock[0] & 0x7) == 5))
                s = -1;

            if ((b.dataBlock[0] & 0x3) == 3 && (a1.dataBlock[0] & 0x3) == 3)
                s = -s;

            if (a1.DataLength == 1 && a1.dataBlock[0] == 1)
                return s;
            else
                return (s * Jacobi(b % a1, a1));
        }



        // definicja metody pobierajacej bity
        public byte[] GetBytes()
        {
            int numBits = CountBytes();

            int numBytes = numBits >> 3;
            if ((numBits & 0x7) != 0)
                numBytes++;

            byte[] result = new byte[numBytes];

            int position = 0;
            uint tempVal, value = dataBlock[DataLength - 1];


            if ((tempVal = (value >> 24 & 0xFF)) != 0)
                result[position++] = (byte)tempVal;

            if ((tempVal = (value >> 16 & 0xFF)) != 0)
                result[position++] = (byte)tempVal;
            else if (position > 0)
                position++;

            if ((tempVal = (value >> 8 & 0xFF)) != 0)
                result[position++] = (byte)tempVal;
            else if (position > 0)
                position++;

            if ((tempVal = (value & 0xFF)) != 0)
                result[position++] = (byte)tempVal;
            else if (position > 0)
                position++;


            for (int i = DataLength - 2; i >= 0; i--, position += 4)
            {
                value = dataBlock[i];
                result[position + 3] = (byte)(value & 0xFF);
                value >>= 8;
                result[position + 2] = (byte)(value & 0xFF);
                value >>= 8;
                result[position + 1] = (byte)(value & 0xFF);
                value >>= 8;
                result[position] = (byte)(value & 0xFF);
            }

            return result;
        }
    }
}