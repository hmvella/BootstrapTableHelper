# BootstrapTableHelper
C# Helper Class for Wenzhixin's Bootstrap Table


## Example
### Controller
```c#
SampleEntities db = new SampleEntities();

public string SelectCustomerData(int pageSize, int pageNumber, string sortOrder, string sortBy, string type, string searchString = "", bool searchStartOnly = false)
{ 
     var queryable = db.vRetailCustomers.AsQueryable(); 
     var results = new BootstrapTableHelper().GenerateTable(queryable, pageSize, pageNumber, sortBy, sortOrder, searchString, searchStartOnly); //So ease. Much wow. return results;
}
```
### View
```html
<div class="row  row-padder">
        <div class="col-xs-12">
            <div id="toolbar">
                <div class="well well-sm">
                    <span class="pseudo-header text-info"><strong>Search: &nbsp;&nbsp;</strong></span>
                    <label class="radio-inline">
                        <input type="radio" name="searchradio" onclick="refreshTable" checked="">Entire Row
                    </label>
                    <label class="radio-inline">
                        <input type="radio" name="searchradio" onclick="refreshTable">First Word
                    </label>
                </div>
            </div>
            <div id="selectCustomer" class="tab-pane fade in active">
                <div class="hidden" id="searchString"></div>
                <div class="hidden" id="sortBy"></div>
                <div class="hidden" id="sortOrder"></div>
                <table data-pagination=true
                       data-classes='table-no-bordered'
                       data-toolbar="#toolbar"
                       data-search=true
                       data-page-number="1"
                       data-page-size="20"
                       data-url="/RetailFrontEnd/SelectCustomerData"
                       data-query-params="setParams"
                       data-side-pagination="server"
                       data-search-on-enter-key="true"
                       data-toggle="table"
                       data-striped=true
                       data-query-params-type="Else"
                       class="table"
                       data-page-list="[10, 25, 50]"
                       data-trim-on-search="false"
                       data-sortable="true"
                       onSort="Sort">
                    <thead class="text-info">
                        <tr class="">
                            <th data-field="Code">
                                Code
                            </th>
                            <th data-field="EntityName" data-sortable="true" >
                                Surname & Name
                            </th>
                            <th data-field="Regno" data-sortable="true">
                                ID Card
                            </th>
                            <th data-field="Address" data-sortable="true">
                                Address
                            </th>
                            <th data-field="Contact" data-sortable="true">
                                Contact
                            </th>
                            <th data-field="Locality" data-sortable="true">
                                Locality
                            </th>
                        </tr>
                    </thead>
                </table>
            </div>
        </div>
    </div>
```

### Script
```JS

function refreshTable() {
    $('table').bootstrapTable('refresh', {
        onSearch: setSearchString(),
        onLoadSuccess: setSearchString(),
        onLoadError: setSearchString()
    });
}

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


function sortBy(name, order) {
    $('#sortBy').attr('name', name);
    $('#sortBy').attr('order', order);
}

$(document).ready(function () {
    $('#selectCustomer').on('sort.bs.table', function (e, name, order) {
        sortBy(name, order);
    });
});
```
