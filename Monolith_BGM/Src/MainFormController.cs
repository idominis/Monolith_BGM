using System;
using System.Collections.Generic;
//using Monolith_BGM.Services;

namespace Monolith_BGM.Controllers
{
    public class MainFormController
    {
        private readonly DataService _dataService;

        public event Action<List<DateTime>> DataInitialized;
        public event Action<string> ErrorOccurred;

        public MainFormController(DataService dataService)
        {
            _dataService = dataService;
        }

        public async void InitializeDataAsync()
        {
            try
            {
                var dates = await _dataService.FetchDistinctOrderDatesAsync();
                DataInitialized?.Invoke(dates);
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke($"Failed to load order dates: {ex.Message}");
            }
        }
    }
}
