namespace ExpressValidator
{
	internal interface IExpressPropertyValidator<TObj, T> :  IExpressPropertyValidatorBase<T>, IObjectValidator<TObj> { }
}
