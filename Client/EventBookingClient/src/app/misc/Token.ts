export const Getrole = (token : string | null) : string =>{
    if(token){
        var payload = JSON.parse(atob(token.split('.')[1]));
        // console.log(payload);
        return payload.role;
    }
    return "Invalid Role";
}

export const GetUserID = (token : string | null) : string =>{
    if(token){
        var payload = JSON.parse(atob(token.split('.')[1]));
        // console.log(payload);
        return payload.nameid;
    }
    return "Invalid Role";
}

