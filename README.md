# BootstrapTableHelper
C# Helper Class for Wenzhixin's Bootstrap Table with Npoco. C# code uses the npoco instead of entity framework. Therefore compared to the previous version there are some small differences.
Small note NP is reference to Npoco.  

## Example
### Controller
```c#
        public string SelectSupplierData(int pageSize, int pageNumber, string sortOrder, string sortBy, string searchString = "", bool searchStartOnly = false)
        {
			//data is the query that is run on the choice of view/table 
            var data = new QueryProvider<V_PL_ACCOUNTS_NP>(db);
			//the helper here calls the np version of the bootstrapTable which requires the data as a query  
            var helper = new NPBootstrapTableHelper();
			//Here the searching and formating of the table 
            var results = helper.GenerateTable(data, pageSize, pageNumber, sortBy, sortOrder, searchString, searchStartOnly, false);
			//returns the results and how the data is going to be formatted.
            return results;
        }
```
### View
```html
<!--Currently using bootstrap 4, there are some small differences compared to bootstap 3-->
<div id="sortBy" by="SUCODE" order="ASC" period="1" class="d-none"></div>
<div class="tab-content">
    <div id="itemList" class="tab-pane fade show active">
        <table data-pagination=true data-classes="table-no-bordered"
               data-search=true
               data-page-number=1
               data-trim-on-search=false
               data-search-on-enter-key=true
               data-page-size="50"
               data-toolbar="#searchBox"
               data-url="/ProductionProto/SelectSupplierData"
               data-sortable=true
               data-query-params="setSupplierParams"
               data-side-pagination="server"
               data-toggle="table"
               data-striped=true
               data-row-style="rowStyle"
               data-query-params-type="Else" class="table"
               data-page-list="[10, 25, 50]">
            <thead class="text-primary">
                <tr>
                    <th data-field="SUNAME">
                        Name
                    </th>
                    <th data-field="SUADDRESS" data-searchable="true">
                        Address
                    </th>
                    <th data-field="SU_COUNTRY_CODE" data-searchable="true">
                        Country
                    </th>
                    <th data-field="SUPHONE" data-searchable="true">
                        Telephone
                    </th>
                    <th data-field="SU_VAT_REG_NO" data-searchable="true">
                        Vat NO
                    </th>
                </tr>
            </thead>
        </table>
    </div>
</div>
```

### Script
```JS
//this method is used to refresh the table
//can be used to get the latest data
function refreshTable() {
    $('table').bootstrapTable('refresh', {
        onSearch: setSearchString(),
        onLoadSuccess: setSearchString(),
        onLoadError: setSearchString()
    });
}
//Used in combition with the Mvc view, it gets the items with the respectivte id
//It gets the params from the view and sets them accordingly to what the user decides
function setParams(params) {

    var sortBy = $('#sortBy').attr('by');
    var sortOrder = $('#sortBy').attr('order')
    params.sortBy = sortBy;
    params.sortOrder = sortOrder;

    var searchType = $('input[name="searchradio"]:checked').parent().text();
    searchType = searchType.replace('\n', '').trim();

    if (searchType === "First Word") {
        params.searchStartOnly = true;
    } else {
        params.searchStartOnly = false;
    }

    var search = $('.search input').val();
    if (search) {
        params.searchString = search;
    }

    if (search) {
        $('#searchString').text(search);
    } else {
        $('#searchString').text('');
    }

    return params;
}

//Shall sort the data based on 'sortBy' label that is hidden on the 
function sortBy(name, order) {
    $('#sortBy').attr('name', name);
    $('#sortBy').attr('order', order);
}
//Is used on loading to load the sortby function 
$(document).ready(function () {
    $('#selectCustomer').on('sort.bs.table', function (e, name, order) {
        sortBy(name, order);
    });
});
```
