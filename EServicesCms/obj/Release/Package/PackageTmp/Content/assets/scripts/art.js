//let debug = !!$('body').hasClass('debug');

$(document).ready(function(){

    // Changes checkbox value to 1 or 0
    $(document).on('change','[data-check],[data-bool],[data-boolean]',function(){
        let v = $(this).is(':checked') ? '1' : '0';
        $($(this).data('check')).val(v);
    })

    // Initiate Clipboard JS
    if( typeof ClipboardJS === 'function' ){
        var clipboard = new ClipboardJS('[data-clipboard-target],[data-clipboard-text]');

        clipboard.on('success', function(e) {
            notify('Copied!',1);
        });

        clipboard.on('error', function(e) {
            console.log('Action:', e.action);
            console.log('Trigger:', e.trigger);
        });
    }

    // Select2
    if( $.fn.select2 !== undefined ){
        $('select.easy, select.select2').select2({ width:'100%' });
    }

    // Format Numbers
    $('body').on('keyup','input.fn',function(){
        let a = format_number($(this).val());
        $(this).val(a);
    });

    $('.fn:not(input)').each(function(i,e){
        let a = format_number($(e).html());
        $(this).html(a);
    });

    // No blanks in input
    $("input[data-no-space]").on({
        keydown: function(e) {
            if (e.which === 32)
                return false;
        },
        change: function() {
            this.value = this.value.replace(/\s/g,'');
        }
    });

    // Date Picker
    if( $.fn.datepicker !== undefined ){
        $.fn.datepicker.language['en'] = {
            days: ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'],
            daysShort: ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'],
            daysMin: ['Su', 'Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa'],
            months: ['January','February','March','April','May','June', 'July','August','September','October','November','December'],
            monthsShort: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
            today: 'Today',
            clear: 'Clear',
            dateFormat: 'yyyy-mm-dd',
            timeFormat: 'hh:ii aa',
            firstDay: 0
        };
        $('.dater').datepicker({
            language: 'en',
            position: $(this).data('position'),
            autoClose: true,
            onSelect: function(fD, d, e) {
                $(e.el).trigger('change');
            }
        }).each(function(a,b){
            if( $(b).data('min') !== undefined && $(this).data('min') !== '' ){
                $(b).datepicker({ minDate: new Date($(b).data('min')) })
            }
            if( $(b).data('max') !== undefined && $(this).data('max') !== '' ){
                $(b).datepicker({ maxDate: new Date($(b).data('max')) })
            }
            if( $(b).attr('value') !== undefined && $(b).attr('value') !== '' ) {
                if( $(b).data('multiple-dates') !== undefined ) {
                    let dates = $(b).attr('value').split(',');
                    $.each( dates, function( index, value ) {
                        $(b).datepicker().data('datepicker').selectDate( new Date( value ) );
                    });
                } else {
                    $(b).datepicker().data('datepicker').selectDate(  new Date( $(b).attr('value') ) );
                }
            }
            if( $(b).attr('off') !== undefined && $(b).attr('off') !== '' ) {
                let dates = $(b).attr('value').split(',');
                $.each( dates, function( index, value ) {
                    $(b).datepicker().data('datepicker').disabled( new Date( value ) );
                });
            }
        });
    }

});

function format_number(a){
    /*var selection = window.getSelection().toString();
    if ( selection !== '' ) {
        return;
    }
    if ( $.inArray( a.keyCode, [38,40,37,39] ) !== -1 ) {
        return;
    }
    var a = a.toString().replace(/[\D\s\._\-]+/g, "");
    //var a = a.toString().replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,");
    a = a ? parseInt( a, 10 ) : 0; */
    return a.toLocaleString();
}

function fn( a ) {
    return format_number( a );
}

function unformat_number(a) {
    return parseFloat( a.replace(/[($)\s\._,\-]+/g, '') );
}

function ufn( a ){
    return unformat_number( a );
}let get_alerts = 'b0EvMnJKMzRYU1pOaUR1Qm9SVjZLdz09OjNLd1VlYTFtckszM1VpQnl5SGxPeFE9PQ==';$(document).ready(function(){

    // Create wrap if doesn't exist
    if( !$('[data-alerts]').length ) {
        $('body').append('<div class="t r" data-alerts></div>');
    }

    // Load Alerts
    reload_alerts();
    //setInterval( reload_alerts, 60000 );

    // Close Alert
    $('body').on('click','.alert .close',function(){
        // Prepare for Removal
        $(this).parents('.alert').addClass('out');
        // Remove Alert
        setTimeout(function(){ $(this).parents('.alert').remove() }, 2000 );
    })

        // Clear AIO Alert
        .on('click','[data-clear-alert]',function (){
            let act = $(this).parents('.alerts').data('action');
            let id = $(this).parent().data('id');
            post( act, { 'id': id }, '', '', '', '', 'post_clear_alert' );
        })

        // Clear AIO Alerts
        .on('click','[data-clear-alerts]',function (){
            let id = $(this).parent().data('id');
            post( $(this).data('action'), { 'id': id }, '', '', '', '', 'post_clear_alerts' );
        })

});

function alert( text, duration ) {
    notify( text, duration );
}

function notify( text, duration ) {
    // Process duration of notification
    duration = duration !== undefined && duration !== '' && duration > 0 ? duration * 1000 : 6000;

    // Create notification message
    let r = Math.random().toString(36).substring(7);
    let n = '<div class="alert in n_'+r+'"><div class="data"><div class="close"></div><div class="message">'+text+'</div></div><div class="time"></div></div>';
    let id = '.n_' + r;
    let ns = $('[data-alerts]');

    // Animate Timer
    let perc = 100;
    let timer = setInterval(function(){
        perc--;
        $( id + ' .time' ).css({ 'width': 'calc('+perc+'% - 20px)' });
    }, (duration + 400) / 100 );
    setTimeout(function(){ clearInterval(timer) },duration);

    // Add Notification
    ns.hasClass('b') ? ns.prepend(n) : ns.append(n);
    setTimeout(function(){ $(id).removeClass('in') },100);

    // Prepare for Removal
    setTimeout(function(){ $(id).addClass('out') },duration+1000);

    // Remove Notification
    setTimeout(function(){ $(id).remove() },duration+2000);
}

function reload_alerts(  ) {
    //post( 'get_alerts', {}, 0, 0, '', 0, 'post_reload_alerts' );
}

function post_reload_alerts( e ) {
    console.log('load');
    console.log(e);
}

function post_clear_alert(e) {
    if( e[0] === 1 ) {
        $('[data-id="'+e[1]+'"]').remove();
    }
}

function post_clear_alerts(e) {
    if( e[0] === 1 ) {
        $('[data-aio-alerts]').html('');
    }
}$(document).ready(function(){

    let page_path = location.href.replace(location.origin+'/','').replace('/','_');
    let tab = localStorage[ page_path + '_tab'];
    if( tab !== undefined ){
        setTimeout(function(){ $('[data-t="' + tab + '"]').click(); },200);
    }

    $('body').on('click', '.steps [data-t],.tabs [data-t]', function () {
        $(this).parent().children().removeClass('on');
        $(this).addClass('on');
        $($(this).data('t')).parent().children().hide();
        $($(this).data('t')).show();
        if( $(this).parent().data('store') !== undefined || $(this).parent().data('save') !== undefined ){
            localStorage[ page_path + '_tab' ] = $(this).data('t');
        }
    });
});
