var parsleyConfig = {
    errorsContainer: function (pEle) {
        var $err = pEle.$element.closest('.form-group').find('.help-block');
        return $err;
    }
}