namespace Books_Store.Models.Pagination
{
    public class Pager
    {
        //number of  total items in the list
        public int TotalItems { get; private set; }

        //number of Current Page Number
        public int CurrentPageNumber { get; private set; }

        //number of items per page
        public int PageSize { get; private set; }

        // number ot Total Pages which ==  ( total items / number of items per page ) =>
        // i have 100 item and want display 2 item on each page
        //1 page => 2 items
        //?? page => 100 items
        // ??== (100*1)/2 == 50 total pages
        public int TotalPages { get; private set; }


        //Start and End Pagination is meanning number of pagination boxes displayed
        //this numbers depend on the number of list items we have
        //and next we control this boxes number by if condition
        //we do this by make loop start from StartPagination number and end at EndPagination number
        //which make box at each cycle
        public int StartPagination { get; private set; }

        public int EndPagination { get; private set; }


        public Pager()
        {

        }
        public Pager(int  totalItems, int _currentPage, int pageSize=10)
        {
            int currentPageNumber = _currentPage;
            int totalPages = (int)Math.Ceiling( (decimal)totalItems / (decimal)pageSize );

            //in default way we want display 10 pagination boxes when items are too much
            //so we want display 4 boxes after current page
            //and display 5 boxes before current page
            //so total pagination boxes= 10 (5(before) + 1(current) + 4(after))
            int startPagination = currentPageNumber - 5;
            int endPagination = currentPageNumber +4;

  

            //true for first  five pages, for ex if current page is 3 => startpag wiil be -2 and endpag will be 7 
            //and ofcourse we want not do this
            if (startPagination <= 0)
            {
                //end = 7 - (-2 -1) == 10 Tadaaa
                endPagination = endPagination - (startPagination - 1);

                //fixed reset it to 1 which is the first page
                startPagination = 1;
            }


            //true for last 3 pages, for ex if we have 100 page and we in the page 97 => startpag wiil be 92,
            //however endpage will be 101 ,and ofcourse we want not do this

            if (endPagination> totalPages)
            {
                //fixed reset it to toatal pages which is the last page
                endPagination = totalPages;

                if (endPagination > 10)//true starting from page number 97 => 97 rest to 100 and startpage is 97-5=92
                    //and by this we loop from 92 untill 100 which genrate 8 boxes just
                    //and we want display 10 boxes
                {
                    startPagination = endPagination - 9; //reset  start to 100-9=91                                                         

                }
            }

            TotalItems= totalItems;
            CurrentPageNumber= currentPageNumber;
            PageSize = pageSize;
            TotalPages= totalPages;
            StartPagination= startPagination;
            EndPagination= endPagination;


        }



    }
}
