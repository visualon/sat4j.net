
namespace Sharpen {

    public class Number {
        object _val;


        public override string ToString(){
            return _val?.ToString();
        }

        public static implicit operator Number(long val){
            return new Number {_val = val};
        }

         public static implicit operator Number(int val){
            return new Number {_val = val};
        }
    }
}
