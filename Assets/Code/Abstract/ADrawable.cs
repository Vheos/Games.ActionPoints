namespace Vheos.Games.ActionPoints
{
    abstract public class ADrawable<T> : ADrawableBase where T : AMatProps, new()
    {
        // Privates
        protected T _matProps;

        // Overrides
        override protected void InitializeMatProps()
        {
            _matProps = new T();
            _matPropsBase = _matProps;
        }
    }
}