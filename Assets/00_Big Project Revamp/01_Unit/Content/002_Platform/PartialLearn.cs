using UnityEngine;

namespace Rush
{
    public partial class PartialLearn // main partial
    {
        private void FunctionA()
        {
            // bagaimana cara saat partial lain dihapus
            // maka function yang dipanggil pada partial part lain tidak error
            FunctionB();
        }
        partial void FunctionB();
    }
    // kalo ini dihapus maka tidak akan ada error pada main partial
    public partial class PartialLearn // second partial
    {
        partial void FunctionB()
        {
            Debug.Log("FunctionB called");
            // write any logic here
        }
        
    }
}
