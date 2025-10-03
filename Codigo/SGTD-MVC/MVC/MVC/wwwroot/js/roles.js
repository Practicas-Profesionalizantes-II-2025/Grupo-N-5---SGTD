// Modal Roles
$('#modalRoles').on('shown.bs.modal', function () {
    $.get(urlsRol.getRoles, function (data) {
        $('#tablaRolesContainer').html(data);
    });
});

// Click en modificar
$(document).on('click', '.btn-modificar', function () {
    const row = $(this).closest('tr');
    const id = row.data('id');
    const nombre = row.find('.nombre-rol').text().trim();

    const form = $('#formCrearRol');
    form.find('input[name="Id"]').val(id);
    form.find('input[name="Nombre"]').val(nombre);
    form.find('button[type="submit"]').text('Guardar');
});

// Enviar formulario
$(document).on('submit', '#formCrearRol', function (e) {
    e.preventDefault();
    const form = $(this);
    const id = form.find('input[name="Id"]').val();
    const url = id ? urlsRol.modificarRol : urlsRol.crearRol;

    $.ajax({
        url: url,
        type: 'POST',
        data: form.serialize(),
        success: function (data) {
            $('#tablaRolesContainer').html(data);
            form[0].reset();
            form.find('button[type="submit"]').text('Agregar');
        },
        error: function (xhr) {
            alert("Error: " + xhr.responseText);
        }
    });
});

//Eliminar rol
$(document).on('click', '#modalRoles .btn-eliminar-rol', function () {
    if (!confirm('¿Eliminar este rol?')) return;

    const row = $(this).closest('tr');
    const id = row.data('id');

    $.ajax({
        url: urlsRolRol.eliminarRol + '/' + id,
        type: 'POST',
        success: function (data) {
            $('#tablaRolesContainer').html(data);
        },
        error: function (xhr) {
            alert("Error al eliminar: " + xhr.responseText);
        }
    });
});
