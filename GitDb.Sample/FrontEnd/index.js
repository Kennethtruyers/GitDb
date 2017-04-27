var map;
var branch = "master";
var markers = [];
var $select;
var $autoRefresh;

function getPinsUrl() {
    return "/pins?branch=" + branch;
}

function onStartUp() {
    $select = $("#selBranches");
    $autoRefresh = $("#chkAutoRefresh");
    loadData();

    setInterval(onAutoTimerElapsed, 3000);
    map = new google.maps.Map(document.getElementById('map'), {
        center: { lat: 40.476245, lng: -3.685434 },
        zoom: 14
    });

    google.maps.event.addListener(map, 'click', onMapClicked);
    $select.on("change", onBranchChanged);

}
function onAutoTimerElapsed() {
    if ($autoRefresh.is(":checked"))
        loadData();
}

function onBranchChanged() {
    branch = $select.val();
    loadData();
}


function loadData() {
    $.get(getPinsUrl())
        .done(function (pins) {
            for (var i = 0; i < markers.length; i++) {
                markers[i].setMap(null);
            }
            markers = [];
            for (var i = 0; i < pins.length; i++) {
                addMarker(pins[i]);
            }

        });

    $.get("/branch")
     .done(function (branches) {
            var $select = $("#selBranches");
            $select.empty();
            for (var i = 0; i < branches.length; i++) {
                $select.append($("<option />").val(branches[i])
                                              .text(branches[i]));
            }
            $select.val(branch);
        });
}

function onMapClicked (event) {
    $.post(getPinsUrl(),
        {
            name: prompt("Enter the name of the pin"),
            lat: event.latLng.lat(),
            lng: event.latLng.lng()
        })
        .done(addMarker);
}

function addMarker(pin)
{
    markers.push(new google.maps.Marker({
        position: pin,
        map: map,
        title: pin.name
    })); 
}

