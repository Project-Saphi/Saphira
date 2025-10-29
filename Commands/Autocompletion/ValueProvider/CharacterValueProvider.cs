using Saphira.Saphi.Api;

namespace Saphira.Commands.Autocompletion.ValueProvider
{
    public class CharacterValueProvider : IValueProvider
    {
        private readonly SaphiApiClient _saphiApiClient;

        public CharacterValueProvider(SaphiApiClient saphiApiClient)
        {
            _saphiApiClient = saphiApiClient;        
        }

        public async Task<List<Value>> GetValuesAsync()
        {
            var values = new List<Value>();
            var response = await _saphiApiClient.GetCharactersAsync();

            if (response?.Success == true)
            {
                foreach (var character in response.Data)
                {
                    var value = new Value(int.Parse(character.Id), character.Name);
                    values.Add(value);
                }
            }

            return values;
        }
    }
}
