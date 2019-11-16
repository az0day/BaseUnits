using System;
using System.Text.RegularExpressions;

namespace BaseUnits.Core.Service
{
    /// <summary>
    /// 服务之间的关联标识
    /// </summary>
    public class CorrelationId : ICorrelationIdLog
    {
        private static readonly Lazy<CorrelationId> CorrelationIdEmptyLazy = new Lazy<CorrelationId>(() =>
        {
            return new CorrelationId
            {
                _value = "EMPTY"
            };
        });

        private string _value;

        /// <summary>
        /// 固定的"空白"标识
        /// </summary>
        public static CorrelationId Empty => CorrelationIdEmptyLazy.Value;

        public readonly DateTime CreatedOn = DateTime.Now;

        /// <summary>
        /// 产生一个随机标识
        /// </summary>
        /// <param name="prefix"></param>
        public CorrelationId(string prefix = "")
        {
            var id = Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[/+=]", "");
            _value = string.IsNullOrWhiteSpace(prefix) ? id : $"{prefix}{id}";
        }

        public static CorrelationId New(string prefix = "")
        {
            return new CorrelationId(prefix);
        }

        public override string ToString()
        {
            return _value;
        }

        public static CorrelationId Parse(string rawValue)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
            {
                throw new ArgumentNullException(nameof(rawValue), "rawValue parameter cannot null or empty.");
            }

            return new CorrelationId
            {
                _value = rawValue
            };
        }

        public bool TryParse(string input, out CorrelationId result)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                result = null;
                return false;
            }

            result = new CorrelationId
            {
                _value = input
            };

            return true;
        }
    }
}
