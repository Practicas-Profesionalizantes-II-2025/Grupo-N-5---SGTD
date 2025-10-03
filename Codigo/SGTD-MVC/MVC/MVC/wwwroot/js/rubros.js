// Modal Rubros
$('#modalRubros').on('shown.bs.modal', function () {
    $.get(urlsRubro.getRubros, function (data) {
        $('#tablaRubrosContainer').html(data);
    });
});

// Click en modificar
$(document).on('click', '.btn-modificar', function () {
    const row = $(this).closest('tr');
    const id = row.data('id');
    const nombre = row.find('.nombre-rubro').text().trim();

    const form = $('#formCrearRubro');
    form.find('input[name="Id"]').val(id);
    form.find('input[name="Nombre"]').val(nombre);
    form.find('button[type="submit"]').text('Guardar');
});

// Enviar formulario
$(document).on('submit', '#formCrearRubro', function (e) {
    e.preventDefault();
    const form = $(this);
    const id = form.find('input[name="Id"]').val();
    const url = id ? urlsRubro.modificarRubro : urlsRubro.crearRubro;

    $.ajax({
        url: url,
        type: 'POST',
        data: form.serialize(),
        success: function (data) {
            $('#tablaRubrosContainer').html(data);
            form[0].reset();
            form.find('button[type="submit"]').text('Agregar');
        },
        error: function (xhr) {
            alert("Error: " + xhr.responseText);
        }
    });
});

// Eliminar rubro
$(document).on('click', '#modalRubros .btn-eliminar-rubro', function () {
    if (!confirm('¿Eliminar este rubro?')) return;

    const row = $(this).closest('tr');
    const id = row.data('id');

    $.ajax({
        url: urlsRubro.eliminarRubro + '/' + id,
        type: 'POST',
        success: function (data) {
            $('#tablaRubrosContainer').html(data);
        },
        error: function (xhr) {
            alert("Error al eliminar: " + xhr.responseText);
        }
    });
});
