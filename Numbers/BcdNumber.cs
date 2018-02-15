using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CaptainLib.Numbers
{
    /// <summary>
    /// Implementation of Binary-coded decimal
    /// </summary>
    public class BcdNumber
    {
        public const int MaxResolution = 1024;

        bool _neg;
        bool _int;
        private byte[] _digits;
        private int _plen;
        private int _len;

        private BcdNumber()
        {
        }

        public int DigitsCount { get { return _len; } }

        public BcdNumber(BcdNumber n)
        {
            _neg = n._neg;
            _int = n._int;
            _digits = new byte[n._len];
            Array.Copy(n._digits, _digits, n._len);
            _len = n._len;
            _plen = n._plen;
        }

        public override string ToString()
        {
            var str = new StringBuilder(_len + 2);
            foreach (var d in _digits)
            {
                str.Insert(0, d.ToString());
            }
            if (!_int)
            {
                str.Insert(str.Length - _plen, '.');
            }
            if (_neg)
                str.Insert(0, '-');
            return str.ToString();
        }

        public BcdNumber(string num)
        {
            if (num.Length < 1)
                return;
            var neg = num[0] == '-';
            if (neg && num.Length < 2)
                return;
            if (neg)
                num = num.Substring(1);
            var parts =  num.Split('.');
            if (parts.Length > 2)
                return;
            num = parts[0];
            var frac = parts.Length  > 1 ? parts[1] : string.Empty;
            var plen = frac.Length;
            var len = num.Length + plen;
            var digits = new byte[len];
            var dig_i = 0;
            for (var i = frac.Length - 1; i >= 0; --i)
            {
                if ((frac[i] & 0xF0) != 0x30)
                {
                    return;
                }
                digits[dig_i++] = (byte)(frac[i] & 0x0F);
            }
            for (var i = num.Length - 1; i >= 0; --i)
            {
                if ((num[i] & 0xF0) != 0x30)
                {
                    return;
                }
                digits[dig_i++] = (byte)(num[i] & 0x0F);
            }

            _neg = neg;
            _int = plen == 0;
            _plen = plen;
            _len = len;
            _digits = digits;
        }

        public static BcdNumber operator -(BcdNumber n)
        {
            var result = new BcdNumber(n);
            result._neg = !result._neg;
            return result;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            var bcd = obj as BcdNumber;
            if (bcd == null)
                return false;
            return Compare(this, bcd) == 0;
        }

        public static BcdNumber operator >>(BcdNumber f, int amount)
        {
            return f.ShiftRight(amount);
        }

        public static BcdNumber operator <<(BcdNumber f, int amount)
        {
            return f.ShiftLeft(amount);
        }

#region Arythmetic operators

        public static BcdNumber operator+(BcdNumber f, BcdNumber s)
        {
            return Add(f, s);
        }

        public static BcdNumber operator *(BcdNumber f, BcdNumber s)
        {
            return Mul(f, s);
        }

        public static BcdNumber operator /(BcdNumber f, BcdNumber s)
        {
            return Div(f, s);
        }

        public static BcdNumber operator -(BcdNumber f, BcdNumber s)
        {
            return Sub(f, s);
        }

#endregion

#region Comparison operators

        public static bool operator !=(BcdNumber f, BcdNumber s)
        {
            return !(f == s);
        }

        public static bool operator ==(BcdNumber f, BcdNumber s)
        {
            if (ReferenceEquals(f, null) && ReferenceEquals(s, null))
                return true;

            return Compare(f, s) == 0;
        }

        public static bool operator >(BcdNumber f, BcdNumber s)
        {
            return Compare(f, s) == 1;
        }

        public static bool operator <(BcdNumber f, BcdNumber s)
        {
            return Compare(f, s) == -1;
        }

        public static bool operator >=(BcdNumber f, BcdNumber s)
        {
            return Compare(f, s) >= 0;
        }

        public static bool operator <=(BcdNumber f, BcdNumber s)
        {
            return Compare(f, s) <= 0;
        }
#endregion

        public static BcdNumber Pow10(int pow)
        {
            if (pow == 0)
                return One;
            else if (pow > 0)
                return One << pow;
            return One >> -pow;
        }

        public static int Compare(BcdNumber f, BcdNumber s)
        {
            if (ReferenceEquals(f, null))
                return -1;
            if (ReferenceEquals(s, null))
                return 1;
            if (f._neg != s._neg)
            {
                return f._neg ? -1 : 1;
            }
            var n = f._neg;
            var fIntLen = f._len - f._plen;
            var sIntLen = s._len - s._plen;
            if (fIntLen != sIntLen)
            {
                if (n)
                    return fIntLen > sIntLen ? -1 : 1;
                else
                    return fIntLen > sIntLen ? 1 : -1;
            }
            var intLen = sIntLen;
            for (var i = intLen - 1; i >= 0; --i)
            {
                if (f._digits[i + f._plen] == s._digits[i + s._plen])
                    continue;
                if(n)
                    return f._digits[i + f._plen] > s._digits[i + s._plen] ? -1 : 1;
                else
                    return f._digits[i + f._plen] > s._digits[i + s._plen] ? 1 : -1;
            }
            // integer parts are the same
            var pLen = Math.Min(f._plen, s._plen);
            for (var i = pLen; i >= 0; --i)
            {
                if (f._digits[i] == s._digits[i])
                    continue;
                if (n)
                    return f._digits[i] == s._digits[i] ? -1 : 1;
                else
                    return f._digits[i] == s._digits[i] ? 1 : -1;
            }
            // shared plen frac parts are the same
            if (f._plen == s._plen)
                return 0;
            if (n)
                return f._plen > s._plen ? -1 : 1;
            else
                return f._plen > s._plen ? 1 : -1;
        }

        private static void SumDigits(byte[] dig1, int off1, byte[] dig2, int off2, ref byte[] digO, int offO, out int pos)
        {
            if (digO == null)
            {
                var len = Math.Max(dig1.Length - off1, dig2.Length - off2);
                digO = new byte[len];
            }
            var len1 = dig1.Length - off1;
            var len2 = dig2.Length - off2;
            if (len2 > len1)
            {
                var digT = dig1;
                var offT = off1;
                var lenT = len1;
                dig1 = dig2;
                off1 = off2;
                len1 = len2;
                dig2 = digT;
                off2 = offT;
                len2 = lenT;
            }
            pos = offO;
            byte carry = 0;
            var i = 0;
            for (; i < len2; ++i)
            {
                int r = dig1[off1 + i] + dig2[off2 + i] + carry;
                if (r > 9)
                {
                    carry = 1;
                    r = r - 10;
                }
                else
                {
                    carry = 0;
                }
                if (pos == digO.Length)
                    Array.Resize(ref digO, digO.Length + 1);
                digO[pos++] = (byte)r;
            }
            while (i < len1)
            {
                int r = dig1[off1 + i++] + carry;
                if (r > 9)
                {
                    carry = 1;
                    r = r - 10;
                }
                else
                {
                    carry = 0;
                }
                if (pos == digO.Length)
                    Array.Resize(ref digO, digO.Length + 1);
                digO[pos++] = (byte)r;
            }
            if (carry > 0)
            {
                Array.Resize(ref digO, digO.Length + 1);
                digO[pos] = carry;
            }
        }

        private static void SortSwap(ref BcdNumber f, ref BcdNumber s)
        {
            if (s._len > f._len)
            {
                var t = s;
                s = f;
                f = t;
            }
        }

        private BcdNumber ShiftLeft(int amount)
        {
            if (amount == 0)
                return this;
            if (IsZero())
                return Zero;
            var plen = _plen - amount;
            if (plen >= 0)
            {
                var result = new BcdNumber(this);
                result._plen = plen;
                result._int = plen == 0;
                return result.Trim();
            }
            amount -= _plen;
            var noZeroesLen = _len;
            while (_digits[noZeroesLen - 1] == 0 && _len > 1) --noZeroesLen;
            var nLen = noZeroesLen + amount;
            var digits = new byte[nLen];
            Array.Copy(_digits, 0, digits, amount, noZeroesLen);
            return new BcdNumber()
            {
                _digits = digits,
                _int = true,
                _plen = 0,
                _len = _len + amount,
                _neg = _neg
            };
        }

        private BcdNumber ShiftRight(int amount)
        {
            if (amount == 0)
                return this;
            if (IsZero())
                return Zero;
            var nLen = 0;
            var intLen = _len - _plen;
            var zero = intLen == 1 && _digits[_plen] == 0;
            if (amount < intLen)
            {
                nLen = _len;
            }
            else
            {
                nLen = _len + amount - intLen + 1;
            }
            var digits = new byte[nLen];
            Array.Copy(_digits, digits, _len);
            return new BcdNumber()
            {
                _digits = digits,
                _len = nLen,
                _plen = _plen + amount,
                _int = false,
                _neg = _neg
            }.Trim();
        }

        private BcdNumber Trim()
        {
            var intLen = _len - _plen;
            var leadingZeroes = 0;
            while (leadingZeroes < intLen - 1 && _digits[_len - 1 - leadingZeroes] == 0) ++leadingZeroes;
            var zeroes = 0;
            if(!_int)
             while (zeroes < _plen && _digits[zeroes] == 0) ++zeroes;
            if (zeroes == 0 && leadingZeroes == 0)
                return this;
            var nLen = _len - zeroes - leadingZeroes;
            if (nLen == 0)
                return Zero;
            var digits = new byte[nLen];
            Array.Copy(_digits, zeroes, digits, 0, digits.Length);

            var plen = _plen - zeroes;
            return new BcdNumber()
            {
                _digits = digits,
                _len = digits.Length,
                _plen = plen,
                _int = plen == 0,
                _neg = _neg
            };
        }

        public bool IsZero()
        {
            return _len == 1 && _digits[0] == 0;
        }

        private static BcdNumber Add(BcdNumber f, BcdNumber s)
        {
            if (f._neg != s._neg)
            {
                if (s._neg)
                    return Sub(f, -s);
                else
                    return Sub(s, -f);
            }
            Align(ref f, ref s);
            var len = Math.Max(f._len, s._len);
            var digits = new byte[len];
            SortSwap(ref f, ref s);
            SumDigits(f._digits, 0, s._digits, 0, ref digits, 0, out int pos);

            var result = new BcdNumber
            {
                _int = f._int && s._int,
                _neg = f._neg && s._neg,
                _digits = digits,
                _len = digits.Length,
                _plen = f._plen
            };

            return result;
        }

        private static BcdNumber Sub(BcdNumber f, BcdNumber s)
        {
            if (f._neg != s._neg)
            {
                if (s._neg)
                    return Add(f, -s);
                else
                    return -Add(-f, s);
            }
            if (f._neg)
            {
                var t = -f;
                f = -s;
                s = t;
            }
            var neg = false;
            var cmp = Compare(f, s);
            if (cmp == 0)
            {
                return Zero;
            }
            if (cmp < 0)
            {
                neg = true;
                var t = f;
                f = s;
                s = t;
            }
            Align(ref f, ref s);
            var digits = new byte[f._len];
            var carry = 0;
            var i = 0;
            for (; i < s._len; ++i)
            {
                var r = f._digits[i] - s._digits[i] - carry;
                if (r < 0)
                {
                    r += 10;
                    carry = 1;
                }
                else
                {
                    carry = 0;
                }
                digits[i] = (byte)r;
            }
            for (; i < f._len; ++i)
            {
                var r = f._digits[i] - carry;
                if (r < 0)
                {
                    r += 10;
                }
                carry = 0;
                digits[i] = (byte)r;
            }
            i = digits.Length - 1;
            while (i >= 0 && digits[i] == 0) --i;
            if(i != digits.Length - 1)
                Array.Resize(ref digits, digits.Length - (digits.Length - i - 1));

            return new BcdNumber()
            {
                _digits = digits,
                _len = digits.Length,
                _int = f._int && s._int,
                _neg = neg,
                _plen = f._plen
            }.Trim();
        }

        private static BcdNumber Div(BcdNumber f, BcdNumber s)
        {
            if (s == Zero)
                throw new DivideByZeroException();
            var neg = f._neg != s._neg;
            var plen = 0;
            var digits = new byte[f._len];
            var dPos = 0;

            var maxPLen = Math.Max(f._plen, s._plen);

            if (maxPLen > 0)
            {
                f = f.ShiftLeft(maxPLen);
                s = s.ShiftLeft(maxPLen);
            }

            do
            {
                var shiftFix = f._digits[f._len - 1] < s._digits[s._len - 1] ? 1 : 0;
                if (f > s)
                {
                    var shift = f._len - s._len - shiftFix;
                    s <<= shift;
                    dPos =+ shift;
                }
                else if(f < s)
                {
                    var shift = s._len - f._len + shiftFix;
                    f <<= shift;
                    dPos -= shift;
                }
                if (dPos < 0)
                {
                    var l = -dPos;
                    Array.Resize(ref digits, digits.Length + l);
                    Array.Copy(digits, 0, digits, l, digits.Length - l);
                    for (var i = 0; i < l; ++i) digits[i] = 0;
                    plen += l;
                    dPos = 0;
                }
                if (digits.Length <= dPos)
                {
                    Array.Resize(ref digits, dPos + 1);
                }
                var r = 0;
                while (f >= s)
                {
                    f -= s;
                    ++r;
                }
                digits[dPos] = (byte)r;

            } while (!f.IsZero() && plen < MaxResolution);

            return new BcdNumber()
            {
                _digits = digits,
                _len = digits.Length,
                _int = plen == 0,
                _neg = neg,
                _plen = plen
            }.Trim();
        }

        private static byte[] MulByte(byte[] dig, byte mul, int offset)
        {
            byte[] rBcd = new byte[2];
            var result = new byte[dig.Length + offset];

            for (var i = 0; i < dig.Length; ++i)
            {
                int r = dig[i] * mul;
                if (r > 9)
                {
                    rBcd[1] = (byte)(r / 10);
                    rBcd[0] = (byte)(r % 10);
                }
                else
                {
                    rBcd[1] = 0;
                    rBcd[0] = (byte)r;
                }
                SumDigits(result, offset + i, rBcd, 0, ref result, offset + i, out int pos);
            }

            return result;
        }

        struct MulByteCacheEntry
        {
            public byte[] Data;
            public int Offset;

            public int Len { get { return Data.Length - Offset; } }
        }

        private static BcdNumber Mul(BcdNumber f, BcdNumber s)
        {
            Align(ref f, ref s);
            var isInt = f._int && s._int;
            var neg = f._neg != s._neg;
            var plen = f._plen + s._plen;
            SortSwap(ref f, ref s);
            var cached = new Dictionary<byte, MulByteCacheEntry>();
            var sumArray = new Queue<byte[]>();

            for (var i = 0; i < s._len; ++i)
            {
                var mul = s._digits[i];
                byte[] r;
                if (cached.TryGetValue(mul, out MulByteCacheEntry fromCache))
                {
                    r = new byte[fromCache.Len + i];
                    Array.Copy(fromCache.Data, fromCache.Offset, r, i, fromCache.Len);
                    sumArray.Enqueue(r);
                }
                else
                {
                    var k = i;
                    var mulByte = MulByte(f._digits, mul, k);
                    cached.Add(mul, new MulByteCacheEntry() { Data = mulByte, Offset = k });
                    sumArray.Enqueue(mulByte);
                }
            }

            cached.Clear();
            var result = SumQueue(sumArray);

            return new BcdNumber()
            {
                _digits = result,
                _len = result.Length,
                _plen = plen,
                _neg = neg,
                _int = isInt
            }.Trim();
        }

        private static byte[] SumQueue(Queue<byte[]> queue)
        {
            while (queue.Count > 1)
            {
                var s1 = queue.Dequeue();
                var s2 = queue.Dequeue();
                byte[] rs = null;
                SumDigits(s1, 0, s2, 0, ref rs, 0, out int pos);
                queue.Enqueue(rs);
            }

            return queue.First();
        }

        private static void Align(ref BcdNumber f, ref BcdNumber s)
        {
            if (f._plen == s._plen)
                return;

            bool isInt1 = f._int;
            int len1 = f._len;
            int plen1 = f._plen;
            var digits1 = new List<byte>(len1);
            BcdNumber result1 = null;

            bool isInt2 = s._int;
            int len2 = s._len;
            int plen2 = s._plen;
            var digits2 = new List<byte>(len2);
            BcdNumber result2 = null;

            if (f._plen > s._plen)
            {
                var c = f._plen - s._plen;
                var zeros = Enumerable.Repeat<byte>(0, c);
                plen2 += c;
                len2 += c;
                isInt2 = false;
                digits2.AddRange(zeros);
                digits2.AddRange(s._digits);
                result1 = f;
            }
            else
            {
                var c = s._plen - f._plen;
                var zeros = Enumerable.Repeat<byte>(0, c);
                plen1 += c;
                len1 += c;
                isInt1 = false;
                digits1.AddRange(zeros);
                digits1.AddRange(f._digits);
                result2 = s;
            }

            if (result1 == null)
            {
                result1 = new BcdNumber
                {
                    _digits = digits1.ToArray(),
                    _neg = f._neg,
                    _int = isInt1,
                    _len = len1,
                    _plen = plen1
                };
            }

            if (result2 == null)
            {
                result2 = new BcdNumber
                {
                    _digits = digits2.ToArray(),
                    _neg = s._neg,
                    _int = isInt2,
                    _len = len2,
                    _plen = plen2
                };
            }

            f = result1;
            s = result2;
        }

        private byte GetDigit(int index)
        {
            return _digits[index];
        }

        private byte SetDigit(int index, byte value)
        {
            var t = _digits[index];
            _digits[index] = value;
            return t;
        }

        static BcdNumber()
        {
            Zero = new BcdNumber("0");
            One = new BcdNumber("1");
        }

        static BcdNumber Zero;
        static BcdNumber One;
    }
}