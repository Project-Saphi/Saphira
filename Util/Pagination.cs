namespace Saphira.Util
{
    public class Pagination
    {
        private readonly int _itemsPerPage;
        private readonly int _currentPage;
        private readonly int _pageSize;

        public Pagination(int itemsPerPage, int currentPage, int totalItemCount)
        {
            _itemsPerPage = itemsPerPage;
            _currentPage = currentPage;
            _pageSize = totalItemCount;
        }
    }
}
