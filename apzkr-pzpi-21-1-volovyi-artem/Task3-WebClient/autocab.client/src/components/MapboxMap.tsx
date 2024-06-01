import mapboxgl from "mapbox-gl";
import "mapbox-gl/dist/mapbox-gl.css";
import React, { useEffect, useState } from "react";


interface MapboxMapProps {
  routeCoordinates: number[][];
  currentLocation: number[];
}

const MapboxMap: React.FC<MapboxMapProps> = ({ currentLocation }) => {
  const [map, setMap] = React.useState<mapboxgl.Map>();
  const [lng, setLng] = useState(0);
  const [lat, setLat] = useState(0);
  const [zoom, setZoom] = useState(12);

  const mapNode = React.useRef(null);

  useEffect(() => {
    const node = mapNode.current;
    if (typeof window === "undefined" || node === null) return;

    const mapboxMap = new mapboxgl.Map({
      container: node,
      accessToken: "pk.eyJ1IjoibG9zZHJldyIsImEiOiJjbHB1eGJkaHgwMHljMmtxeng2NzA4dndxIn0.S4r1YfGRASP85mHPYNjZuQ",
            style: "mapbox://styles/mapbox/streets-v11",
      center: currentLocation,
      zoom: 12
    });

    setMap(mapboxMap);
    
    mapboxMap.on('load', () => {
      mapboxMap.on('move', () => {
        setLng(mapboxMap.getCenter().lng.toFixed(4));
        setLat(mapboxMap.getCenter().lat.toFixed(4));
        setZoom(mapboxMap.getZoom().toFixed(2));
      });

      new mapboxgl.Marker().setLngLat(currentLocation).addTo(mapboxMap);
    });

    return () => {
      mapboxMap.remove();
    };
  }, []);

  return ( 
    <div>
      <div className="sidebar">
        Longitude: {lng} | Latitude: {lat} | Zoom: {zoom}
      </div>
      <div ref={mapNode} style={{ width: "500px", height: "500px" }} />
    </div>
  );
};

export default MapboxMap;