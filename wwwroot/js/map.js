let map;
let directionsService;
let directionsRenderer;
let pickupMarker;
let destinationMarker;

function initMap() {
    const kyiv = { lat: 50.4501, lng: 30.5234 };

    map = new google.maps.Map(document.getElementById('map'), {
        zoom: 12,
        center: kyiv
    });

    directionsService = new google.maps.DirectionsService();
    directionsRenderer = new google.maps.DirectionsRenderer({
        map: map,
        suppressMarkers: false
    });

    const pickupInput = document.getElementById('PickupAddress');
    const destinationInput = document.getElementById('DestinationAddress');

    if (pickupInput && destinationInput) {
        const pickupAutocomplete = new google.maps.places.Autocomplete(pickupInput);
        const destAutocomplete = new google.maps.places.Autocomplete(destinationInput);

        pickupAutocomplete.addListener('place_changed', () => {
            const place = pickupAutocomplete.getPlace();
            if (place.geometry) {
                calculateRoute();
            }
        });

        destAutocomplete.addListener('place_changed', () => {
            const place = destAutocomplete.getPlace();
            if (place.geometry) {
                calculateRoute();
            }
        });
    }
}

function calculateRoute() {
    const pickupAddress = document.getElementById('PickupAddress').value;
    const destinationAddress = document.getElementById('DestinationAddress').value;

    if (!pickupAddress || !destinationAddress) return;

    const request = {
        origin: pickupAddress,
        destination: destinationAddress,
        travelMode: google.maps.TravelMode.DRIVING
    };

    directionsService.route(request, (result, status) => {
        if (status === 'OK') {
            directionsRenderer.setDirections(result);

            const route = result.routes[0].legs[0];
            const distanceInKm = (route.distance.value / 1000).toFixed(2);

            document.getElementById('Distance').value = distanceInKm;

            calculatePrice();

            document.getElementById('routeInfo').innerHTML = `
                <div class="alert alert-info mt-3">
                    <strong>Відстань:</strong> ${route.distance.text}<br>
                    <strong>Час у дорозі:</strong> ${route.duration.text}
                </div>
            `;
        } else {
            alert('Не вдалося побудувати маршрут: ' + status);
        }
    });
}

function calculatePrice() {
    const serviceSelect = document.getElementById('ServiceId');
    const distanceInput = document.getElementById('Distance');
    const priceEstimate = document.getElementById('priceEstimate');
    const estimatedPrice = document.getElementById('estimatedPrice');

    const serviceId = parseInt(serviceSelect.value);
    const distance = parseFloat(distanceInput.value) || 0;

    if (serviceId && distance > 0) {
        fetch(`/api/services/${serviceId}`)
            .then(response => response.json())
            .then(service => {
                const totalPrice = service.basePrice + (service.pricePerKm * distance);
                priceEstimate.style.display = 'block';
                estimatedPrice.textContent = `${totalPrice.toFixed(2)} грн`;
            });
    }
}
