const apiUrl = 'http://localhost:5298/api/Servicios'; // Cambia con la URL de tu API
let editingServicioId = null;

// Cargar todos los servicios al iniciar la página
window.onload = function() {
    fetchServicios();
};

// Función para obtener todos los servicios con opción de búsqueda
function fetchServicios(nombre = "", pageNumber = 1, pageSize = 10) {
    fetch(`${apiUrl}?nombre=${nombre}&pageNumber=${pageNumber}&pageSize=${pageSize}`)
        .then(response => response.json())
        .then(data => {
            const serviciosList = document.getElementById('serviciosList');
            serviciosList.innerHTML = '';
            data.forEach(servicio => {
                serviciosList.innerHTML += `
                    <tr>
                        <td>${servicio.servicioID}</td>
                        <td>${servicio.nombreServicio}</td>
                        <td>${servicio.descripcion}</td>
                        <td>${servicio.precio}</td>
                        <td>
                            <button class="btn btn-warning" onclick="editServicio(${servicio.servicioID})">Editar</button>
                            <button class="btn btn-danger" onclick="deleteServicio(${servicio.servicioID})">Eliminar</button>
                        </td>
                    </tr>
                `;
            });
        });
}

// Función para buscar servicios por nombre
function searchServicios() {
    const searchValue = document.getElementById('searchInput').value;
    fetchServicios(searchValue);
}

// Registrar nuevo servicio
document.getElementById('registerButton').addEventListener('click', function(event) {
    event.preventDefault();

    const formData = {
        nombreServicio: document.getElementById('nombreServicio').value,
        descripcion: document.getElementById('descripcion').value,
        precio: document.getElementById('precio').value
    };

    fetch(apiUrl, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(formData)
    }).then(response => {
        if (response.ok) {
            resetForm();
            fetchServicios();
            document.getElementById('registerResult').innerHTML = '<div class="alert alert-success">Servicio registrado con éxito.</div>';
        } else {
            document.getElementById('registerResult').innerHTML = '<div class="alert alert-danger">Error al registrar el servicio.</div>';
        }
    });
});

// Función para cargar datos del servicio en el formulario para editar
function editServicio(id) {
    fetch(`${apiUrl}/${id}`)
        .then(response => response.json())
        .then(servicio => {
            document.getElementById('nombreServicio').value = servicio.nombreServicio;
            document.getElementById('descripcion').value = servicio.descripcion;
            document.getElementById('precio').value = servicio.precio;
            editingServicioId = servicio.servicioID;

            document.getElementById('registerButton').style.display = 'none';
            document.getElementById('updateButton').style.display = 'inline-block';
            document.getElementById('cancelButton').style.display = 'inline-block';
        });
}

// Actualizar servicio
document.getElementById('updateButton').addEventListener('click', function(event) {
    event.preventDefault();

    const formData = {
        servicioID: editingServicioId,
        nombreServicio: document.getElementById('nombreServicio').value,
        descripcion: document.getElementById('descripcion').value,
        precio: document.getElementById('precio').value
    };

    fetch(`${apiUrl}/${editingServicioId}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(formData)
    }).then(response => {
        if (response.ok) {
            resetForm();
            fetchServicios();
            document.getElementById('registerResult').innerHTML = '<div class="alert alert-success">Servicio actualizado con éxito.</div>';
        } else {
            document.getElementById('registerResult').innerHTML = '<div class="alert alert-danger">Error al actualizar el servicio.</div>';
        }
    });
});

// Eliminar servicio
function deleteServicio(id) {
    if (confirm('¿Estás seguro de que deseas eliminar este servicio?')) {
        fetch(`${apiUrl}/${id}`, {
            method: 'DELETE'
        }).then(response => {
            if (response.ok) {
                fetchServicios();
                document.getElementById('registerResult').innerHTML = '<div class="alert alert-success">Servicio eliminado con éxito.</div>';
            } else {
                document.getElementById('registerResult').innerHTML = '<div class="alert alert-danger">Error al eliminar el servicio.</div>';
            }
        });
    }
}

// Función para resetear el formulario
function resetForm() {
    document.getElementById('servicioForm').reset();
    editingServicioId = null;

    document.getElementById('registerButton').style.display = 'inline-block';
    document.getElementById('updateButton').style.display = 'none';
    document.getElementById('cancelButton').style.display = 'none';
}

// Función para cancelar la edición
document.getElementById('cancelButton').addEventListener('click', function() {
    resetForm();
});
