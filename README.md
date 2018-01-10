# BootstrapTableHelper
C# Helper Class for Wenzhixin's Bootstrap Table

    public string SelectCustomerData(int pageSize, int pageNumber, string sortOrder, string sortBy, string type, string searchString = "", bool searchStartOnly = false)
     { 
	     var queryable = db.vRetailCustomers.AsQueryable(); 
	     var results = new BootstrapTableHelper().GenerateTable(queryable, pageSize, pageNumber, sortBy, sortOrder, searchString, searchStartOnly); //So ease. Much wow. return results;
      }
