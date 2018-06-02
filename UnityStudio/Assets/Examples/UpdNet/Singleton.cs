namespace VRNetLibrary
{
    public class Singleton<T> where T : new()
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                return Singleton<T>.instance;
            }
        }

        static Singleton()
        {
            Singleton<T>.instance = new T();
        }

        protected Singleton()
        {
        }
    }
}
