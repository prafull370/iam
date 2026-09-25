var baseUrl = ($('base').attr('href').slice(0, -1));
function decodeBase64(value) {
    if (!value) return null;

    try {
        let decoded = atob(value);

        if (/^[A-Za-z0-9+/=]+$/.test(decoded) && decoded.endsWith('=')) {
            try { decoded = atob(decoded); } catch { }
        }
        return decoded;
    } catch {
        return value;
    }
}


    $(document).ready(function () {
        const params = new URLSearchParams(window.location.search);

        //const action = params.get("action") ? atob(params.get("action")) : null;
        //const id = params.get("id") ? atob(params.get("id")) : null;
        //const discom_id = params.get("discom_id") ? atob(params.get("discom_id")) : null;
        const action = decodeBase64(params.get("action"));
        const id = decodeBase64(params.get("id"));
        const discom_id = decodeBase64(params.get("discom_id"));


        
        //const action = params.get("action");
        //const id = params.get("id");
        //const discom_id = params.get("discom_id");

        if (action && id) {
            $('#main').css('margin-left', '0px');
            if (action === "Product") {
                bindProductTab(id);
            } else if (action === "Service") {
                bindServiceTab(id, discom_id);
            } else if (action === "Module") {
                bindModuleTab(id);
            }
        }
        else {
            // Only call bindDiscom when no action parameter
            if (window.location.pathname === "/Home/Index") {
                localStorage.clear();
                $('#main').css('margin-left', '0px');
                bindDiscom();
            } else {
                $('#main').css('margin-left', '');
            }
            const path = window.location.pathname.toLowerCase();

            if (path.includes('iam') && !path.includes('home/index')) {
                bindPage();
            }
            //if (window.location.href.toLowerCase().includes('iam')) {
            //    bindPage();
            //}
        }
    });

function bindProduct(productId) {
    const actionEnc = btoa("Product");
    const idEnc = btoa(productId.toString());
    const url = window.location.origin + window.location.pathname + "?action=" + actionEnc + "&id=" + idEnc;
    window.open(url, "_self");
}

function bindService(serviceId, projecturl) {
    const params = new URLSearchParams(window.location.search);
    let id = params.get("id");
    if (id === null || id === undefined || id === "") {
        id = $("#hdnDiscomId").val();
    }
    let actionEnc = btoa("Service");
    let serviceidEnc = btoa(serviceId.toString());
    let discomidEnc = btoa(id.toString());
    const baseUrl = window.location.origin;
    if (baseUrl.toLocaleLowerCase() === projecturl.toLocaleLowerCase().replace(/\/+$/, '') || projecturl==="") {
        let url = window.location.origin + window.location.pathname + "?action=" + actionEnc + "&id=" + serviceidEnc + "&discom_id=" + discomidEnc;
        window.open(url, "_self");
    }
    else {
        let url = projecturl;
        window.open(url, "_blank");
    }
    
}

function bindModule(moduleId, serviceurl) {
    let actionEnc = btoa("Module");
    let moduleidEnc = btoa(moduleId.toString());
    const baseUrl = window.location.origin;
    if (baseUrl.toLocaleLowerCase() === serviceurl.toLocaleLowerCase().replace(/\/+$/, '') || serviceurl === "") {
        let url = window.location.origin + window.location.pathname + "?action=" + actionEnc +"&id=" + moduleidEnc;
        window.open(url, "_self");
    }
    else {
        let url = serviceurl;
        window.open(url, "_blank");
    }
}

// BindDiscom
function bindDiscom() {
    $.ajax({
        url: baseUrl + "/Home/GetDiscom",
        dataType: "html",
        beforeSend: function () {
            $("#dvProcessing").show();
        },
        success: function (_html) {
            $("#dvProcessing").hide();
            $('#discomContainer').html(_html);
            const discoms = $('#discomContainer').find('.discom-item'); // Assuming each has class 'discom-item'
            if (discoms.length === 1) {
                const discomId = discoms.first().data('id'); // Use data-id on the HTML
                bindProductTab(discomId); // Auto-load Products
            }
        },
        error: function (e) {
            showAlert(e.statusText, "Warning", "Ok");
        }
    })
}

// BindProduct
function bindProductTab(discomId) {
    $("#dvProcessing").show();
    $.ajax({
        url: baseUrl + "/Home/GetProduct",
        dataType: "html",
        data: {
            Discom_Id: discomId
        },
        beforeSend: function () {
            $("#dvProcessing").show();
            $('#discomContainer').html('');
        },
        success: function (_html) {
            $("#dvProcessing").hide();
            $("#hdnDiscomId").val(discomId);
            $('#productContainer').html(_html);
            
            const products = $('#productContainer').find('.product-item'); // Assuming each has class 'product-item'
            if (products.length === 1) {
                const productId = products.first().data('id'); // Use data-id on the HTML
                $("#hdnProductId").val(productId);  // add value in the hidden product_id
                const projecturl = products.first().data('url'); // Use data-id on the HTML
                let baseUrl = window.location.origin;
                if (baseUrl.toLocaleLowerCase() === projecturl.toLocaleLowerCase().replace(/\/+$/, '') || projecturl === "") {
                    bindServiceTab(productId, discomId); // Auto-load services
                }
                else {
                    window.open(projecturl, "_blank");
                }
                
            }
        },
        error: function (e) {
            showAlert(e.statusText, "Warning", "Ok");
        }
    })
}

// BindService
function bindServiceTab(productId, discom_id) {
    
    $("#dvProcessing").show();
    $.ajax({
        url: baseUrl + "/Home/GetService",
        dataType: "html",
        data: {
            Discom_id: discom_id,
            Product_Id: productId
        },
        beforeSend: function () {
            $("#dvProcessing").show();
            $('#discomContainer').html('');
            $('#productContainer').html('');
            $('#serviceContainer').html('');
        },
        success: function (_html) {
            $("#dvProcessing").hide();
            $('#serviceContainer').html(_html);
            const services = $('#serviceContainer').find('.service-item'); // Assuming each has class 'service-item'
            sessionStorage.setItem("PRODUCT_ID", btoa(productId.toString()));
            if (services.length === 1) {
                const serviceId = services.first().data('id'); // Use data-id on the HTML
                const serviceurl = services.first().data('url'); // Use data-id on the HTML
                let baseUrl = window.location.origin;
                if (baseUrl.toLocaleLowerCase() === serviceurl.toLocaleLowerCase().replace(/\/+$/, '') || serviceurl === "") {
                    bindModuleTab(serviceId, productId); // Auto-load module
                }
                else {
                    window.open(serviceurl, "_blank");
                }
                
            }
        },
        error: function (e) {
            showAlert(e.statusText, "Warning", "Ok");
        }
    })
}

// Bind Module
function bindModuleTab(serviceId, product_id = null) {
    product_id = product_id || atob(sessionStorage.getItem("PRODUCT_ID"));
    $("#dvProcessing").show();
    $.ajax({
        url: baseUrl + "/Home/GetModule",
        dataType: "html",
        data: {
            Service_Id: serviceId
        },
        beforeSend: function () {
            $("#dvProcessing").show();
            $('#discomContainer').html('');
            $('#productContainer').html('');
            $('#serviceContainer').html('');
            $('#moduleContainer').html('');
        },
        success: function (_html) {
            $("#dvProcessing").hide();
            $('#moduleContainer').html(_html);
            const modules = $('#moduleContainer').find('.module-item'); // Assuming each has class 'service-item'
            if (modules.length === 0) {
                bindPages(serviceId, 'Service', product_id); // Auto-load Pages
            }
            else if (modules.length === 1) {
                const moduleId = modules.first().data('id'); // Use data-id on the HTML
                bindPages(moduleId, 'Module', product_id); // Auto-load Pages
            }
        },
        error: function (e) {
            showAlert(e.statusText, "Warning", "Ok");
        }
    })
}

function bindPages(id, flag, product_id = null) {
    sessionStorage.setItem("MENU_FLAG", btoa(flag.toString()));
    sessionStorage.setItem("MENU_ID", btoa(id.toString()));
    sessionStorage.setItem("Product_ID", btoa(product_id.toString()));
    $.ajax({
        url: baseUrl + "/Home/BindPage",
        dataType: "html",
        data: {
            Id: id,
            Flag: flag,
            Product_Id: product_id
        },
        beforeSend: function () {
            var _html = '<div class="customLoader">' +
                '<img src="' + baseUrl + '/images/loader.svg")" />' +
                '</div>';
            $('#sidemenu').html(_html);
        },
        success: function (model) {
            //$('#sidemenu').html(model);

            var landingPage = $('<div>').html(model).find('#landingPageUrl').val();
            setTimeout(function () {

                if (landingPage) {
                    window.location.href = landingPage;
                }
            }, 100);
            localStorage.setItem("sidebarHtml", model);
        },
        error: function (e) {
            showAlert(e.statusText, "Warning", "Ok");
        }
    });
}


function redirectToLandingPage(landingPageUrl) {
    if (landingPageUrl && !localStorage.getItem("hasRedirectedToLandingPage")) {
        localStorage.setItem("hasRedirectedToLandingPage", "true");
        window.location.href = landingPageUrl;
    }
}




function bindPage() {
    let flag = atob(sessionStorage.getItem("MENU_FLAG")); 
    let id = atob(sessionStorage.getItem("MENU_ID"));
    let product_id = atob(sessionStorage.getItem("PRODUCT_ID"));
    $.ajax({
        url: baseUrl + "/Home/BindPage",
        dataType: "html",
        data: {
            Id: id,
            Flag: flag,
            Product_Id: product_id
        },
        beforeSend: function () {
            var _html = '<div class="customLoader">' +
                '<img src="' + baseUrl + '/images/loader.svg")" />' +
                '</div>';
            $('#sidemenu').html(_html);
            $('#sidebar').removeClass('d-none');
        },
        success: function (model) {
            $('#sidemenu').html(model);
            var landingPage = $('#sidemenu').find('#landingPageUrl').val();
            var childLength = $('#sidemenu').find('.nav-item').length;
            if (childLength > 0) {
                $('#sidebar').removeClass('d-none');
            }
            else {
                $('#sidebar').addClass('d-none');
            }
            if (landingPage && !localStorage.getItem('landingRedirected')) {
                localStorage.setItem('landingRedirected', 'true');
            }
        },
        error: function (e) {
            showAlert(e.statusText, "Warning", "Ok");
        }
    });
}
