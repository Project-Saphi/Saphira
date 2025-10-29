using Saphira.Saphi.Api;

namespace Saphira.Commands.Autocompletion.ValueProvider
{
    public class CategoryValueProvider : IValueProvider
    {
        private readonly SaphiApiClient _saphiApiClient;

        public CategoryValueProvider(SaphiApiClient saphiApiClient)
        {
            _saphiApiClient = saphiApiClient;        
        }

        public async Task<List<Value>> GetValuesAsync()
        {
            var values = new List<Value>();
            var response = await _saphiApiClient.GetCategoriesAsync();

            if (response?.Success == true)
            {
                foreach (var category in response.Data)
                {
                    var value = new Value(int.Parse(category.Id), category.Name);
                    values.Add(value);
                }
            }

            return values;
        }
    }
}
