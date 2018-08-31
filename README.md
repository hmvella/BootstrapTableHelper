# BootstrapTableHelper
Helper classes for simple integration of [Wenzhixin's Bootstrap Table](http://bootstrap-table.wenzhixin.net.cn/) with ASP.NET MVC Entity Framework or NPoco ORMs when using Server Side pagination. Provides support for dynamic sorting, searching and pagination. 

Use BootstrapHelper.cs for EntityFramework;
Use NPBootstrapHelper.cs for Npoco; 

## Example
Follow the below example across your Controller, Razor/HTML, and Javascript. 
### Controllers
#### When using Entity Framework
```c#
public string SelectCustomerData(int pageSize, int pageNumber, string sortOrder, string sortBy, string type, string searchString = "", bool searchStartOnly = false)
{ 
     var queryable = db.V_PL_ACCOUNTS.AsQueryable(); 
     var helper =  new BootstrapTableHelper();
     var results = helper.GenerateTable(queryable, pageSize, pageNumber, sortBy, sortOrder, searchString, searchStartOnly); //So ease. Much wow. 
     
     return results;
}
```
#### When using NPoco
```c#
public string SelectSupplierData(int pageSize, int pageNumber, string sortOrder, string sortBy, string searchString = "", bool 	searchStartOnly = false)
{
    var data = new QueryProvider<V_PL_ACCOUNTS_NP>(db);
    var helper = new NPBootstrapTableHelper();
    var results = helper.GenerateTable(data, pageSize, pageNumber, sortBy, sortOrder, searchString, searchStartOnly, false); //So ease. Much wow.

    return results;
}
```
### HTML or Razor View
The critical attributes are: data-side-pagination, data-url, data-field and data-query-params. See Wenzhixin's documentation for JS counterparts.
Use the c# class properties as the data-field values.
```html
<!--This example is using Bootstrap 4 - For bootstrap 3, switch classes 'd-none' with 'hidden' and 'show' with 'in'  -->
<div id="sortBy" by="SUCODE" order="ASC" period="1" class="d-none"></div>
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
		    <th data-field="SUNAME" data-sortable="true" data-searchable="true">
			Name
		    </th>
		    <th data-field="SUADDRESS" data-sortable="true" data-searchable="true">
			Address
		    </th>
		    <th data-field="SU_COUNTRY_CODE" data-sortable="true"  data-searchable="true">
			Country
		    </th>
		    <th data-field="SUPHONE"  data-sortable="true" data-searchable="true">
			Telephone
		    </th>
		    <th data-field="SU_VAT_REG_NO"  data-sortable="true" data-searchable="true">
			Vat NO
		    </th>
		</tr>
	    </thead>
	</table>
</div>
```

### JavaScript
```js
//Retrieve sort key, sort order, search parameters from view, and pass along with default params.
function setSupplierParams(params) {

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

//Required to store sort key and order
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
