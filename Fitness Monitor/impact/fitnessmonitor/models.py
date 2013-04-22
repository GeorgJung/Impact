from django.db import models
from django.contrib.auth.models import User,UserManager

# Create your models here.
class Practitioner(User):
    level = models.IntegerField(null=True, blank=True,default=0)

    def __unicode__(self):
        return self.first_name

class Session(models.Model):
    trainee = models.ForeignKey(Practitioner)
    start = models.DateTimeField(null=True)
    end = models.DateTimeField(null=True)

class Round(models.Model):
    trainee = models.ForeignKey(Practitioner)
    session = models.ForeignKey(Session)
    start = models.DateTimeField(null=True)
    end = models.DateTimeField(null=True)
    # duration