var baseUrl = ($('base').attr('href').slice(0, -1));
var pagingCSS = "<'tbl-scroll1'tr><'dtpg'<'dtpginn'<'dtinfo'i><'dtcntrls'p>>>";
var pagingType = "cstmpaging";
var pageLength = 15;
var bDestroy = true;
var bAutoWidth = false;
var processing = true;
var serverSide = true;
var filter = true;
var responsive = true;
var language = {
    "processing": '<img src="' + baseUrl + '/images/loader.svg" alt="loader" class="loaderImg"/>',
    searchPlaceholder: 'Search...',
    paginate: {
        next: '<i class="ugo-icons"> chevron_right </i>', 
        previous: '<i class="ugo-icons"> chevron_left </i>'
    },
    // emptyTable: `<div style="text-align: center;">
    //                         <img src="/images/no_data.svg" alt="No Data" style="width: 250px; height: 250px; margin-bottom: 10px;">
    //                         <p style="font-size: 15px">No records available at the moment.</p>
    //                      </div>`
};
