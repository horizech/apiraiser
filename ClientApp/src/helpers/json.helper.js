export function normalizeJson(str){
    return str.replace(/[\s\n\r\t]/gs, '').replace(/,([}\]])/gs, '$1')
    .replace(/([,{\[]|)(?:("|'|)([\w_\- ]+)\2:|)("|'|)(.*?)\4([,}\]])/gs, (str, start, q1, index, q2, item, end) => {
        item = item.replace(/"/gsi, '').trim();
        if(index){index = '"'+index.replace(/"/gsi, '').trim()+'"';}
        if(!item.match(/^[0-9]+(\.[0-9]+|)$/) && !['true','false'].includes(item)){item = '"'+item+'"';}
        if(index){return start+index+':'+item+end;}
        return start+item+end;
    });
}