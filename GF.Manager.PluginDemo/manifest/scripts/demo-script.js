(function ($enums) {

    var demoTypeEnum = {};
    demoTypeEnum[demoTypeEnum['Type1'] = 0] = '类型1';
    demoTypeEnum[demoTypeEnum['Type3'] = 1] = '类型2';
    demoTypeEnum[demoTypeEnum['Type3'] = 2] = '类型3';

    $enums.demoTypeEnum = demoTypeEnum;
})($enums || ($enums = {}));