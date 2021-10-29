namespace MyCode
{
    public class MyTestClass1
    {
		private readonly int _p1;
		private readonly double _p2;
		
		public MyTestClass1(int param)
        {
			_p1 = param;
		}

        public MyTestClass1(int p1, double p2)
        {
			_p1 = p1;
			_p2 = p2;
		}

        public MyTestClass1()
        { 
		}
		
        public string GetName()
        {
            return "ThisIsName";
        }

        public double DoSum(double param1, double param2)
        {
			return param1 + param2;
		}

    }


    public interface IMyTestInterface
    { 
	}

    public class MyTestClass2
    {
		private IMyTestInterface _interface;
		
		public MyTestClass2(IMyTestInterface interfaceParam)
        {
			_interface = interfaceParam;
		}
		
        public IMyTestInterface GetInterface()
        {
            return _interface;
        }

        public void SetInterface(IMyTestInterface interfaceParam)
        {
			_interface = interfaceParam;
		}
    }
}