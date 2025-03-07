namespace CoreService.Models.Exceptions
{
    public class NotEnoughMoney : ExceptionToResponseProxy
    {
        public NotEnoughMoney() : base("На счете недостаточно денег для списания", false)
        {
            this.StatusCode = System.Net.HttpStatusCode.UnprocessableEntity;
        }
    }
}
