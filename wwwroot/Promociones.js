const apiUrl = 'http://localhost:5298/api/Promociones';
let editingPromocionId = null; // Para saber si estamos editando una promoción

// Obtener todas las promociones al cargar la página
window.onload = function() {
    fetchPromociones();
};

// Función para obtener todas las promociones
function fetchPromociones(pageNumber = 1, pageSize = 10, codigoPromocional = '', estado = '') {
    const queryString = `?pageNumber=${pageNumber}&pageSize=${pageSize}&codigo=${codigoPromocional}&activo=${estado}`;
    fetch(`${apiUrl}${queryString}`)
        .then(response => response.json())
        .then(data => {
            const promocionesList = document.getElementById('promocionesList');
            promocionesList.innerHTML = '';

            data.forEach(promocion => {
                promocionesList.innerHTML += `
                    <tr>
                        <td>${promocion.promocionID}</td>
                        <td>${promocion.codigoPromocional || 'N/A'}</td>
                        <td>${promocion.descripcion}</td>
                        <td>${promocion.descuentoPorcentaje}</td>
                        <td>${new Date(promocion.fechaInicio).toLocaleDateString()}</td>
                        <td>${new Date(promocion.fechaFin).toLocaleDateString()}</td>
                        <td>${promocion.activo ? 'Activo' : 'Inactivo'}</td>
                        <td>
                            <button class="btn btn-warning" onclick="editPromocion(${promocion.promocionID})">Editar</button>
                            <button class="btn btn-danger" onclick="deletePromocion(${promocion.promocionID})">Eliminar</button>
                        </td>
                    </tr>
                `;
            });
        })
        .catch(error => {
            console.error('Error al obtener promociones:', error);
        });
}

// Función para registrar o actualizar promoción
document.getElementById('registerButton').addEventListener('click', function(event) {
    event.preventDefault();
    submitPromocion('POST');
});

document.getElementById('updateButton').addEventListener('click', function(event) {
    event.preventDefault();
    submitPromocion('PUT');
});

// Función compartida para registrar o actualizar una promoción
function submitPromocion(method) {
    const promocionData = {
        promocionID: editingPromocionId || 0,
        codigoPromocional: document.getElementById('codigoPromocional').value,
        descripcion: document.getElementById('descripcion').value,
        descuentoPorcentaje: parseFloat(document.getElementById('descuentoPorcentaje').value),
        fechaInicio: document.getElementById('fechaInicio').value,
        fechaFin: document.getElementById('fechaFin').value
    };

    if (!promocionData.codigoPromocional || !promocionData.descripcion || isNaN(promocionData.descuentoPorcentaje)) {
        alert("Por favor, completa todos los campos correctamente.");
        return;
    }

    const url = method === 'POST' ? apiUrl : `${apiUrl}/${editingPromocionId}`;

    fetch(url, {
        method: method,
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(promocionData)
    })
    .then(response => {
        if (response.ok) {
            resetForm();
            fetchPromociones();
            document.getElementById('resultMessage').innerHTML = `<div class="alert alert-success">Promoción ${method === 'POST' ? 'registrada' : 'actualizada'} con éxito.</div>`;
        } else {
            response.json().then(data => {
                console.error('Error al registrar/actualizar la promoción:', data);
                document.getElementById('resultMessage').innerHTML = '<div class="alert alert-danger">Error al registrar/actualizar la promoción. Revisa la consola para más detalles.</div>';
            });
        }
    })
    .catch(error => console.error('Error al registrar/actualizar la promoción:', error));
}

// Cargar los datos de la promoción en el formulario para editar
function editPromocion(id) {
    fetch(`${apiUrl}/${id}`)
        .then(response => response.json())
        .then(promocion => {
            document.getElementById('codigoPromocional').value = promocion.codigoPromocional;
            document.getElementById('descripcion').value = promocion.descripcion;
            document.getElementById('descuentoPorcentaje').value = promocion.descuentoPorcentaje;
            document.getElementById('fechaInicio').value = promocion.fechaInicio.split('T')[0];
            document.getElementById('fechaFin').value = promocion.fechaFin.split('T')[0];
            editingPromocionId = promocion.promocionID;

            document.getElementById('registerButton').style.display = 'none';
            document.getElementById('updateButton').style.display = 'inline-block';
            document.getElementById('cancelButton').style.display = 'inline-block';
        });
}

// Eliminar promoción
function deletePromocion(id) {
    if (confirm('¿Estás seguro de que deseas eliminar esta promoción?')) {
        fetch(`${apiUrl}/${id}`, {
            method: 'DELETE'
        }).then(response => {
            if (response.ok) {
                fetchPromociones();
                document.getElementById('resultMessage').innerHTML = '<div class="alert alert-success">Promoción eliminada con éxito.</div>';
            } else {
                document.getElementById('resultMessage').innerHTML = '<div class="alert alert-danger">Error al eliminar la promoción.</div>';
            }
        });
    }
}

// Función para resetear el formulario
function resetForm() {
    document.getElementById('promocionForm').reset();
    editingPromocionId = null;

    document.getElementById('registerButton').style.display = 'inline-block';
    document.getElementById('updateButton').style.display = 'none';
    document.getElementById('cancelButton').style.display = 'none';
}

// Función para cancelar la edición
document.getElementById('cancelButton').addEventListener('click', function() {
    resetForm();
});
