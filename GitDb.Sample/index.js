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

function onMapClicked(event) {
    var name = prompt("Enter the name of the pin");
    if (!name) return;
    $.post(getPinsUrl(), {
            name: name,
            lat: event.latLng.lat(),
            lng: event.latLng.lng()
        })
        .done(addMarker);
}

function onMarkerClicked() {
    var name = prompt("Enter the new name", this.data.name);
    if (name && name !== this.data.name) {
        this.data.name = name;
        $.ajax({
            url: getPinsUrl(),
            type: "PUT",
            data: this.data
        });
    }
}
function onMarkerRightClicked() {
    var marker = this;
    if (confirm("Are you sure you want to delete pin " + marker.data.name)) {
        $.ajax({
            url: getPinsUrl() + "&id=" + marker.data.id,
            type: "DELETE"
        }).done(function() {
            removeMarker(marker);
        });
    }
}

function loadData() {
    $.get(getPinsUrl())
        .done(function (pins) {
            markers.forEach(removeMarker);
            pins.forEach(addMarker);
        });

    $.get("/branch")
        .done(function (branches) {
            $select.empty();
            branches.map(b => $("<option />").val(b).text(b))
                .forEach(o => $select.append(o));
            $select.val(branch);
        });
}

function addMarker(pin) {
    var marker = new google.maps.Marker({
        position: pin,
        map: map,
        data: pin
    });
    marker.addListener('rightclick', onMarkerRightClicked);
    marker.addListener('click', onMarkerClicked);
    markers.push(marker); 
}

function removeMarker(marker) {
    marker.setMap(null);
    var index = markers.indexOf(marker);
    if (index > -1) {
        markers.splice(index, 1);
    }
}