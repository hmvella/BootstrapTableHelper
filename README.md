# BootstrapTableHelper
C# Helper Class for Wenzhixin's Bootstrap Table

<div class="code">
<span class="Modifier">public</span>&nbsp;<span class="ReferenceType">string</span>&nbsp;SelectCustomerData(<span class="ValueType">int</span>&nbsp;pageSize,&nbsp;<span class="ValueType">int</span>&nbsp;pageNumber,&nbsp;<span class="ReferenceType">string</span>&nbsp;sortOrder,&nbsp;<span class="ReferenceType">string</span>&nbsp;sortBy,&nbsp;<span class="ReferenceType">string</span>&nbsp;type,&nbsp;<span class="ReferenceType">string</span>&nbsp;searchString&nbsp;=&nbsp;<span class="String">""</span>,&nbsp;<span class="ValueType">bool</span>&nbsp;searchStartOnly&nbsp;=&nbsp;<span class="Keyword">false</span>)&nbsp;{<br />
&nbsp;<span class="Linq">var</span>&nbsp;queryable&nbsp;=&nbsp;db.vRetailCustomers.AsQueryable();<br />
&nbsp;<span class="Linq">var</span>&nbsp;results&nbsp;=&nbsp;<span class="Keyword">new</span>&nbsp;BootstrapTableHelper().GenerateTable(queryable,&nbsp;pageSize,&nbsp;pageNumber,&nbsp;sortBy,&nbsp;sortOrder,&nbsp;searchString,&nbsp;searchStartOnly);&nbsp;<span class="InlineComment">//So&nbsp;ease.&nbsp;Much&nbsp;wow.</span><br />
<br />
&nbsp;<span class="Statement">return</span>&nbsp;results;<br />
}
</div>
