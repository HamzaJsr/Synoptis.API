using System.ComponentModel.DataAnnotations;


namespace Synoptis.API.Validation
{
    public class FutureDateAttribute : ValidationAttribute
    {
        //Ici je vais override la methode IsValid de ValidationAttribute pour l'adapter a ma sauce

        public override bool IsValid(object? value)
        {
            // donc la on vas utiliser du Pattern Matching ca veut dire on prend value on regarde if value == DateTime ? si oui cree une variable local 
            // date et met dedans la valeur caster de a caster c conversion le type d'une variable donc (DateTime)a c'est comme faire DateTime date = (DateTime)a
            // donc si c'est true dans le if ca cree une variable date dans le scope de la methode IsValid et donc on peut retu date > Date.Time.Now si la date donne par le client est plus ancienne que la date 
            //actuel bah on renvoie false et ca renvoie 
            if (value is not DateTime date)
            {
                return false;
            }

            return date >= DateTime.Now;
        }
    }
}