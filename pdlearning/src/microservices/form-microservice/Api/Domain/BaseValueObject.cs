using System.Text.Json;

namespace Microservice.Form.Domain
{
    public abstract class BaseValueObject
    {
        public static bool operator ==(BaseValueObject val1, BaseValueObject val2)
        {
            return val1.Equals(val2);
        }

        public static bool operator !=(BaseValueObject val1, BaseValueObject val2)
        {
            return !val1.Equals(val2);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            }

            return JsonSerializer.Serialize(this) == JsonSerializer.Serialize(obj);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }

        protected static bool EqualOperator(BaseValueObject left, BaseValueObject right)
        {
            if (left != null && right != null)
            {
                return left.Equals(right);
            }
            else if ((left == null && right != null) || (left != null && right == null))
            {
                return false;
            }

            return true;
        }

        protected static bool NotEqualOperator(BaseValueObject left, BaseValueObject right)
        {
            return !EqualOperator(left, right);
        }
    }
}
