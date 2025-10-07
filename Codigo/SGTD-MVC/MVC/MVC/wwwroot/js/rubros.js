// --- UTILIDADES RUBROS ---
function ensureCancelButtonRubro(form) {
    let $cancel = form.find('.btn-cancel-rubro');
    if ($cancel.length === 0) {
        $cancel = $('<button type="button" class="btn btn-eliminar btn-cancel-rubro ms-2">Cancelar</button>');
        form.find('button[type="submit"]').after($cancel);
        $cancel.on('click', function () {
            resetFormToCreateModeRubro(form);
        });
    }
    $cancel.show();
}

function resetFormToCreateModeRubro(form) {
    form[0].reset();
    form.find('input[name="Id"]').val('');
    form.find('button[type="submit"]').text('Agregar');
    const $cancel = form.find('.btn-cancel-rubro');
    if ($cancel.length) $cancel.hide();
}

// --- Modal Rubros: cargar tabla ---
$('#modalRubros').on('shown.bs.modal', function () {
    $.get(urlsRubro.getRubros, function (data) {
        $('#tablaRubrosContainer').html(data);
        resetFormToCreateModeRubro($('#formCrearRubro')); // Asegurarse de que el formulario esté en modo agregar al abrir
    });
});

// --- Click en modificar ---
$(document).on('click', '.btn-modificar-rubro', function () {
    const row = $(this).closest('tr');
    const id = row.data('id');
    const nombre = row.find('.nombre-rubro').text().trim();

    const form = $('#formCrearRubro');
    form.find('input[name="Id"]').val(id);
    form.find('input[name="Nombre"]').val(nombre);
    form.find('button[type="submit"]').text('Guardar');

    ensureCancelButtonRubro(form);
});

// --- Enviar formulario ---
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
            resetFormToCreateMode(form); // Volver al modo agregar después de guardar
        },
        error: function (xhr) {
            alert("Error: " + xhr.responseText);
        }
    });
});

// --- Eliminar rubro ---
$(document).on('click', '#modalRubros .btn-eliminar-rubro', function () {
    if (!confirm('¿Eliminar este rubro?')) return;

    const row = $(this).closest('tr');
    const id = row.data('id');

    $.ajax({
        url: urlsRubro.eliminarRubro + '/' + id,
        type: 'POST',
        success: function (data) {
            $('#tablaRubrosContainer').html(data);
            resetFormToCreateMode($('#formCrearRubro')); // Asegurarse de resetear el formulario si estaba en edición
        },
        error: function (xhr) {
            alert("Error al eliminar: " + xhr.responseText);
        }
    });
});
